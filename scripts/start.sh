#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "${BASH_SOURCE[0]}")/.."

log() {
  echo "[start] $1"
}

wait_for_port() {
  local port=$1
  local name=$2
  local attempts=30

  while [[ $attempts -gt 0 ]]; do
    if lsof -iTCP:"$port" -sTCP:LISTEN -P -n >/dev/null 2>&1; then
      return 0
    fi
    sleep 1
    attempts=$((attempts - 1))
  done

  echo "ERROR: $name did not start on port $port. Check logs/${name}.log"
  return 1
}

start_detached() {
  local name=$1
  local pidfile=$2
  local logfile=$3
  shift 3

  log "Starting ${name}..."
  setsid "$@" > "$logfile" 2>&1 < /dev/null &
  echo $! > "$pidfile"
}

log "Stopping any previous app processes..."
bash scripts/stop.sh

log "Clearing logs and temp..."
rm -rf logs temp
mkdir -p logs temp

log "Building apps..."
bash scripts/build.sh --no-client

log "Seeding database..."
dotnet run --project src/common/database/SqlLiteDatabase.csproj --no-build --no-launch-profile

start_detached "API on port 5133" logs/api.pid logs/api.log \
  dotnet run --project src/api/Api.csproj --no-build --urls http://0.0.0.0:5133
wait_for_port 5133 "api"

start_detached "worker process" logs/worker.pid logs/worker.log \
  dotnet run --project src/worker/WorkerProcess.csproj --no-build

start_detached "client on port 4200" logs/client.pid logs/client.log \
  npm --prefix src/client run start -- --host 0.0.0.0 --port 4200 --allowed-hosts --watch=false --live-reload=false
wait_for_port 4200 "client"

log "Startup complete. Logs: logs/api.log, logs/worker.log, logs/client.log"
