# Data Pipeline

Angular 20 + .NET 10 monorepo for live coding interviews. Opens in GitHub Codespaces with the Dev Container tooling preconfigured.

## How to use this repo:

- Log into your GitHub account and navigate to the repository root. Make sure the `main` branch is selected.
- Click the `<> Code` button and select `Codespaces`.
- Click `Create codespace on main` and then open your newly created development environment.

## Structure

- [`scripts/`](scripts/) — build / start / stop helpers
- [`src/client/`](src/client/) — Angular 20 app (`ng serve`, port 4200)
- [`src/api/`](src/api/) — .NET 10 Web API (`dotnet run`, port 5133)
- [`src/worker/`](src/worker/) — .NET background worker process
- [`src/common/core/`](src/common/core/) — shared .NET models + service-bus contracts
- [`src/common/database/`](src/common/database/) — EF DbContext + seed console app

## Build

**Ctrl+Shift+B** / **Cmd+Shift+B** runs the default build task (`scripts/build.sh` / `npm run build`):

- `dotnet build` for Core, API, Database, and Worker
- `ng build` (development) for the client

## Start / Stop

Start the apps (stop previous, build, reseed DB, API + Angular) with any of:

- **Terminal → Run Task… → Start app**
- `npm start` or `./scripts/start.sh`

Stop them with:

- **Terminal → Run Task… → Stop app**
- `npm stop` or `./scripts/stop.sh`

Logs go to `logs/`. Forwarded ports 5133 (API) and 4200 (client) appear in the Ports tab.

## Running pieces manually

```bash
dotnet run --project src/common/database
dotnet run --project src/api
dotnet run --project src/worker
```

```bash
npm --prefix src/client start
```
