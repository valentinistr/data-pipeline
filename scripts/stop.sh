#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "${BASH_SOURCE[0]}")/.."
mkdir -p logs

log() {
  echo "[stop] $1"
}

stop_pidfile() {
  local pidfile=$1
  if [[ ! -f "$pidfile" ]]; then
    return
  fi

  local pid
  pid=$(<"$pidfile" 2>/dev/null || true)
  rm -f "$pidfile"

  if [[ -z "${pid:-}" ]]; then
    return
  fi

  if ! kill -0 "$pid" 2>/dev/null; then
    return
  fi

  log "Stopping pid $pid from $pidfile"
  # setsid puts the process in its own group — kill the group when possible
  kill -TERM -- "-$pid" 2>/dev/null || kill -TERM "$pid" 2>/dev/null || true

  for _ in $(seq 1 20); do
    kill -0 "$pid" 2>/dev/null || return
    sleep 0.25
  done

  log "Force-killing pid $pid"
  kill -KILL -- "-$pid" 2>/dev/null || kill -KILL "$pid" 2>/dev/null || true
}

stop_port() {
  local port=$1
  if ! command -v lsof >/dev/null 2>&1; then
    return
  fi

  local pids
  pids=$(lsof -tiTCP:"$port" -sTCP:LISTEN 2>/dev/null || true)
  if [[ -z "$pids" ]]; then
    return
  fi

  for pid in $pids; do
    local cmd
    cmd=$(ps -p "$pid" -o args= 2>/dev/null || true)
    log "Stopping port $port listener ($pid): $cmd"
    kill -TERM "$pid" 2>/dev/null || true
  done

  sleep 0.5

  pids=$(lsof -tiTCP:"$port" -sTCP:LISTEN 2>/dev/null || true)
  for pid in $pids; do
    log "Force-killing port $port listener ($pid)"
    kill -KILL "$pid" 2>/dev/null || true
  done
}

stop_matching() {
  local pattern=$1
  if ! command -v pkill >/dev/null 2>&1; then
    return
  fi
  if pgrep -f "$pattern" >/dev/null 2>&1; then
    log "Stopping processes matching: $pattern"
    pkill -TERM -f "$pattern" 2>/dev/null || true
    sleep 0.5
    pkill -KILL -f "$pattern" 2>/dev/null || true
  fi
}

log "Stopping frontend and backend..."

stop_pidfile logs/api.pid
stop_pidfile logs/consumer.pid
stop_pidfile logs/client.pid

stop_matching "dotnet-watch.dll"
stop_matching "dotnet run --project src/api"
stop_matching "dotnet run --project src/consumer"
stop_matching "dotnet run --project src/database"
stop_matching "ng serve"
stop_matching "npm --prefix src/client run start"

stop_port 5133
stop_port 4200

log "Stopped."
