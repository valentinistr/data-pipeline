#!/usr/bin/env bash
set -euo pipefail

source "$(dirname "${BASH_SOURCE[0]}")/common.sh"

# Fallbacks for processes started outside start.sh, e.g. a manual `dotnet watch` or
# `npm --prefix src/client start` in a terminal. Matching the project directory rather
# than the .csproj path catches both `--project src/backend/Api` and `.../Api.csproj`.
PROCESS_PATTERNS=(
  "dotnet watch --project $API_DIR"
  "dotnet run --project $API_DIR"
  "$API_DIR/bin/"
  "ng serve"
  "npm --prefix $CLIENT_DIR"
)

# Never fails: a process can exit between being listed and being inspected, and under
# `set -o pipefail` that would otherwise abort the script mid-cleanup.
process_group() {
  ps -o pgid= -p "$1" 2>/dev/null | tr -d '[:space:]' || true
}

own_group=$(process_group $$)

# Everything start.sh launches is a process group leader, and a watcher plus the app it
# spawned share that group, so signalling the group stops them as a unit.
signal_group() {
  local signal=$1
  local pid=$2
  local group
  group=$(process_group "$pid")

  if [[ -n $group && $group != "$own_group" ]]; then
    kill -"$signal" -- "-$group" 2>/dev/null && return 0
  fi

  kill -"$signal" "$pid" 2>/dev/null || true
}

# Pidfiles cover the normal case; ports and command lines catch leftovers from an earlier
# session whose pidfiles were cleared.
collect_pids() {
  local name pidfile pid port pattern

  for name in api client; do
    pidfile="$LOG_DIR/$name.pid"
    if [[ -f $pidfile ]]; then
      pid=$(<"$pidfile")
      rm -f "$pidfile"
      echo "$pid"
    fi
  done

  for port in "${APP_PORTS[@]}"; do
    port_listener_pids "$port"
  done

  if command -v pgrep >/dev/null 2>&1; then
    for pattern in "${PROCESS_PATTERNS[@]}"; do
      pgrep -f "$pattern" || true
    done
  fi
}

alive_pids() {
  local pid
  for pid in "$@"; do
    if kill -0 "$pid" 2>/dev/null; then
      echo "$pid"
    fi
  done
}

mapfile -t candidates < <(collect_pids | sort -un)

targets=()
for pid in "${candidates[@]}"; do
  # Never signal this script, its caller (start.sh), or anything sharing our group.
  # The group test is skipped when ps gave us nothing, so an unknown group cannot
  # silently match every candidate.
  if [[ ! $pid =~ ^[0-9]+$ ]] || ((pid == $$ || pid == PPID)); then
    continue
  fi
  if ! kill -0 "$pid" 2>/dev/null; then
    continue
  fi
  if [[ -n $own_group && $(process_group "$pid") == "$own_group" ]]; then
    continue
  fi
  targets+=("$pid")
done

if ((${#targets[@]} == 0)); then
  log "Nothing to stop."
else
  for pid in "${targets[@]}"; do
    log "Stopping $pid: $(ps -o args= -p "$pid" 2>/dev/null | cut -c1-90)"
    signal_group TERM "$pid"
  done

  remaining=()
  for _ in $(seq 1 20); do
    mapfile -t remaining < <(alive_pids "${targets[@]}")
    if ((${#remaining[@]} == 0)); then
      break
    fi
    sleep 0.25
  done

  for pid in "${remaining[@]}"; do
    log "Force-killing $pid"
    signal_group KILL "$pid"
  done
fi

mapfile -t busy < <(busy_ports)
if ((${#busy[@]} > 0)); then
  die "still listening on ports: ${busy[*]}"
fi

log "Stopped."
