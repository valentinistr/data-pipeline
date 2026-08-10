#!/usr/bin/env bash
# Shared configuration and helpers for build.sh, start.sh, and stop.sh.
# Sourced, never executed. Sourcing it puts the caller in the repo root.

cd "$(dirname "${BASH_SOURCE[0]}")/.."

SOLUTION=src/backend/data-pipeline.sln
API_DIR=src/backend/Api
API_PROJECT="$API_DIR/Api.csproj"
DOTNET_PROJECTS=(
  src/backend/Core/Core.csproj
  "$API_PROJECT"
  src/backend/Database/Database.csproj
  src/backend/DataIngestion/DataIngestion.csproj
)
CLIENT_DIR=src/client
RUNTIME_DIR=.runtime
LOG_DIR="$RUNTIME_DIR/logs"
TEMP_DIR="$RUNTIME_DIR/temp"
UPLOADS_DIR="$TEMP_DIR/uploads"
DOTNET_CONFIGURATION=Debug
API_PORT=5133
CLIENT_PORT=4200
APP_PORTS=("$API_PORT" "$CLIENT_PORT")
# Codespaces forwards ports from outside the container, so bind to every interface.
BIND_ADDRESS=0.0.0.0

SCRIPT_LABEL=$(basename "${BASH_SOURCE[1]:-common.sh}" .sh)

log() {
  echo "[$SCRIPT_LABEL] $*"
}

fail() {
  echo "[$SCRIPT_LABEL] ERROR: $*" >&2
  return 1
}

die() {
  fail "$*" || exit 1
}

# True when any of the listed files is newer than the reference. Missing candidates are
# ignored; a missing reference counts as out of date, since -nt is true when only the
# candidate exists.
any_newer() {
  local reference=$1
  shift

  local candidate
  for candidate in "$@"; do
    if [[ -f $candidate && $candidate -nt $reference ]]; then
      return 0
    fi
  done

  return 1
}

# ss (iproute2) is cheaper than lsof, but fall through to lsof rather than trusting a
# single tool: a false negative here would stall start.sh and hide leftovers from stop.sh.
port_is_listening() {
  local port=$1

  if command -v ss >/dev/null 2>&1 && [[ -n $(ss -ltnH "sport = :$port" 2>/dev/null) ]]; then
    return 0
  fi
  if command -v lsof >/dev/null 2>&1; then
    lsof -iTCP:"$port" -sTCP:LISTEN -P -n >/dev/null 2>&1
    return
  fi

  return 1
}

port_listener_pids() {
  command -v lsof >/dev/null 2>&1 || return 0
  lsof -tiTCP:"$1" -sTCP:LISTEN 2>/dev/null || true
}

busy_ports() {
  local port
  for port in "${APP_PORTS[@]}"; do
    if port_is_listening "$port"; then
      echo "$port"
    fi
  done
}

wait_for_port() {
  local port=$1
  local name=$2
  local attempts=${3:-30}

  while ((attempts-- > 0)); do
    if port_is_listening "$port"; then
      return 0
    fi
    sleep 1
  done

  die "$name did not start on port $port. Check $LOG_DIR/$name.log"
}
