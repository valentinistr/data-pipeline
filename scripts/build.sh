#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "${BASH_SOURCE[0]}")/.."

echo "Restoring dependencies..."
dotnet restore src/pf-data-pipeline.sln
echo ".NET dependencies completed."
npm --prefix src/client ci
echo "Client dependencies completed."

echo "Building .NET solution..."
dotnet build src/pf-data-pipeline.sln --no-restore
echo ".NET build completed."

echo "Building Angular client..."
npm --prefix src/client run build -- --configuration development
echo "Client build completed."

echo "Build complete."
