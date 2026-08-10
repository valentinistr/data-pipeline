#!/usr/bin/env bash
set -euo pipefail

source "$(dirname "${BASH_SOURCE[0]}")/common.sh"

BUILD_CLIENT=1
for arg in "$@"; do
  case "$arg" in
    --no-client) BUILD_CLIENT=0 ;;
    *)
      echo "Usage: $0 [--no-client]" >&2
      exit 1
      ;;
  esac
done

# Anything that can change the dependency graph. Files that do not exist are ignored.
RESTORE_INPUTS=(
  "$SOLUTION"
  "${DOTNET_PROJECTS[@]}"
  src/backend/Directory.Build.props
  src/backend/Directory.Packages.props
  src/backend/nuget.config
  src/backend/NuGet.config
)

# Restore only when a project has no restore output yet, or when an input is newer than it.
needs_dotnet_restore() {
  local project assets
  for project in "${DOTNET_PROJECTS[@]}"; do
    assets="${project%/*}/obj/project.assets.json"
    if [[ ! -f $assets ]] || any_newer "$assets" "${RESTORE_INPUTS[@]}"; then
      return 0
    fi
  done

  return 1
}

build_backend() {
  if needs_dotnet_restore; then
    log "Restoring .NET packages..."
    dotnet restore "$SOLUTION"
  else
    log "Skipping .NET restore (up to date)."
  fi

  log "Building .NET solution ($DOTNET_CONFIGURATION)..."
  dotnet build "$SOLUTION" --no-restore -c "$DOTNET_CONFIGURATION"
}

# npm writes node_modules/.package-lock.json on install, so comparing it against the
# lockfile tells us whether the installed tree still matches.
install_client() {
  if any_newer "$CLIENT_DIR/node_modules/.package-lock.json" "$CLIENT_DIR/package-lock.json"; then
    log "Installing client packages..."
    npm --prefix "$CLIENT_DIR" ci --no-audit --no-fund
  else
    log "Skipping npm ci (node_modules up to date)."
  fi
}

build_client() {
  install_client

  log "Building Angular client..."
  npm --prefix "$CLIENT_DIR" run build -- --configuration development
}

# The backend and client steps are independent, so run them together and report both
# outcomes rather than stopping at the first failure.
job_pids=()
job_names=()

start_job() {
  local name=$1
  shift

  "$@" &
  job_pids+=("$!")
  job_names+=("$name")
}

wait_for_jobs() {
  local index status=0
  for index in "${!job_pids[@]}"; do
    if ! wait "${job_pids[index]}"; then
      fail "${job_names[index]} failed." || status=1
    fi
  done

  return "$status"
}

start_job "backend build" build_backend
if ((BUILD_CLIENT)); then
  start_job "client build" build_client
else
  # start.sh passes --no-client: ng serve builds the client itself, but it still needs
  # node_modules to exist.
  start_job "client install" install_client
fi

wait_for_jobs
log "Build complete."
