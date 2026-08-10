#!/usr/bin/env bash
set -euo pipefail

source "$(dirname "${BASH_SOURCE[0]}")/common.sh"

WATCH=1
for arg in "$@"; do
  case "$arg" in
    --no-watch) WATCH=0 ;;
    *)
      echo "Usage: $0 [--no-watch]" >&2
      exit 1
      ;;
  esac
done

started=0

cleanup() {
  local status=$?
  trap - EXIT
  if ((started && status != 0)); then
    log "Startup failed; stopping launched processes..."
    bash scripts/stop.sh || true
  fi
  exit "$status"
}

# setsid moves the process into its own group so it survives this script and so stop.sh
# can take it down together with whatever it spawned.
start_detached() {
  local name=$1
  shift

  log "Starting $name..."
  setsid "$@" > "$LOG_DIR/$name.log" 2>&1 < /dev/null &
  echo $! > "$LOG_DIR/$name.pid"
}

bash scripts/stop.sh

log "Clearing runtime artifacts..."
rm -rf "$RUNTIME_DIR"
mkdir -p "$LOG_DIR" "$UPLOADS_DIR"

# --no-client: ng serve builds the client itself, so only the backend is built here.
# Restore and npm ci are skipped when they are already up to date.
bash scripts/build.sh --no-client

# --no-launch-profile: launchSettings.json applicationUrl would override ASPNETCORE_URLS
api_env=(
  ASPNETCORE_ENVIRONMENT=Development
  ASPNETCORE_URLS="http://$BIND_ADDRESS:$API_PORT"
)
api_command=(dotnet run --project "$API_PROJECT" --no-build --no-launch-profile -c "$DOTNET_CONFIGURATION")
client_args=(--host "$BIND_ADDRESS" --port "$CLIENT_PORT" --allowed-hosts)

if ((WATCH)); then
  api_env+=(DOTNET_WATCH_RESTART_ON_RUDE_EDIT=1 DOTNET_WATCH_SUPPRESS_BROWSER_REFRESH=1)
  # --no-restore: build.sh restored already, so watch reuses that instead of repeating it
  api_command=(dotnet watch --project "$API_PROJECT" --non-interactive --no-restore --no-launch-profile -c "$DOTNET_CONFIGURATION")
  api_attempts=90
else
  client_args+=(--watch=false --live-reload=false)
  api_attempts=30
fi

trap cleanup EXIT

# Both servers build on startup and neither needs the other to be up yet, so start them
# together and wait afterwards.
start_detached api env "${api_env[@]}" "${api_command[@]}"
start_detached client npm --prefix "$CLIENT_DIR" run start -- "${client_args[@]}"
started=1

wait_for_port "$API_PORT" api "$api_attempts"
wait_for_port "$CLIENT_PORT" client 60

trap - EXIT
log "API on :$API_PORT, client on :$CLIENT_PORT. Logs in $LOG_DIR/."
if ((WATCH)); then
  log "Hot reload is on for both."
  log "To debug, run 'Attach to API' in Run and Debug and pick the 'Api' process."
fi
