#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "${BASH_SOURCE[0]}")/.."

usage() {
  echo "Usage: $0 [--no-client]" >&2
}

SKIP_CLIENT=0
for arg in "$@"; do
  case "$arg" in
    --no-client) SKIP_CLIENT=1 ;;
    *)
      echo "Unknown argument: $arg" >&2
      usage
      exit 1
      ;;
  esac
done

needs_dotnet_restore() {
  local assets=(
    src/common/core/obj/project.assets.json
    src/api/obj/project.assets.json
    src/common/database/obj/project.assets.json
    src/worker/obj/project.assets.json
  )
  local f
  for f in "${assets[@]}"; do
    [[ -f "$f" ]] || return 0
  done
  return 1
}

run_dotnet() {
  if needs_dotnet_restore; then
    echo "Restoring .NET dependencies..."
    dotnet restore src/pf-data-pipeline.sln
    echo ".NET dependencies completed."
  else
    echo "Skipping .NET restore (already restored)."
  fi

  echo "Building .NET solution..."
  dotnet build src/pf-data-pipeline.sln --no-restore
  echo ".NET build completed."
}

install_client() {
  if [[ ! -d src/client/node_modules ]]; then
    echo "Installing client dependencies..."
    npm --prefix src/client ci --no-audit --no-fund
    echo "Client dependencies completed."
  else
    echo "Skipping client install (node_modules present)."
  fi
}

run_client() {
  install_client

  echo "Building Angular client..."
  npm --prefix src/client run build -- --configuration development
  echo "Client build completed."
}

wait_jobs() {
  local left_pid=$1
  local left_name=$2
  local right_pid=$3
  local right_name=$4
  local failed=0

  if ! wait "$left_pid"; then
    echo "ERROR: ${left_name} failed." >&2
    failed=1
  fi
  if ! wait "$right_pid"; then
    echo "ERROR: ${right_name} failed." >&2
    failed=1
  fi

  [[ $failed -eq 0 ]]
}

run_dotnet &
dotnet_pid=$!

if [[ "$SKIP_CLIENT" -eq 1 ]]; then
  # start.sh uses this: install npm deps so ng serve can start, but skip ng build
  install_client &
  client_pid=$!
  wait_jobs "$dotnet_pid" ".NET restore/build" "$client_pid" "client install" || exit 1
else
  run_client &
  client_pid=$!
  wait_jobs "$dotnet_pid" ".NET restore/build" "$client_pid" "Angular restore/build" || exit 1
fi

echo "Build complete."
