#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "${BASH_SOURCE[0]}")/.."
mkdir -p logs temp
export DOTNET_USE_POLLING_FILE_WATCHER=1

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

log "Stopping any previous app processes..."
bash scripts/stop.sh

log "Clearing logs and temp..."
rm -rf logs temp
mkdir -p logs temp

log "Building apps..."
bash scripts/build.sh

log "Seeding database..."
dotnet run --project src/database/SqlLiteDatabase.csproj --no-launch-profile

log "Starting API on port 5133..."
setsid dotnet watch run --non-interactive --project src/api/Api.csproj --urls http://0.0.0.0:5133 \
  > logs/api.log 2>&1 < /dev/null &
echo $! > logs/api.pid
wait_for_port 5133 "api"

log "Starting worker process..."
setsid dotnet watch run --non-interactive --project src/consumer/WorkerProcess.csproj \
  > logs/worker.log 2>&1 < /dev/null &
echo $! > logs/worker.pid

log "Starting client on port 4200..."
setsid npm --prefix src/client run start -- --host 0.0.0.0 --port 4200 --allowed-hosts --poll 2000 \
  > logs/client.log 2>&1 < /dev/null &
echo $! > logs/client.pid
wait_for_port 4200 "client"

log "Startup complete. Logs: logs/api.log, logs/worker.log, logs/client.log"
