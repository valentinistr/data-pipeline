# Live Coding Exercise

Angular 20 + .NET 10 monorepo for live coding interviews. Opens in GitHub Codespaces with the Dev Container tooling preconfigured.

## Structure

- [`scripts/`](scripts/) — build / start / stop helpers
- [`src/core/`](src/core/) — shared .NET models + service-bus contracts
- [`src/client/`](src/client/) — Angular 20 app (`ng serve`, port 4200)
- [`src/api/`](src/api/) — .NET 10 Web API (`dotnet run`, port 5133)
- [`src/database/`](src/database/) — EF DbContext + seed console app
- [`src/consumer/`](src/consumer/) — .NET console worker (placeholder)

## Build

**Ctrl+Shift+B** / **Cmd+Shift+B** runs the default build task (`scripts/build.sh` / `npm run build`):

- `dotnet build` for Core, API, Database, and Consumer
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
dotnet run --project src/database
dotnet run --project src/api
dotnet run --project src/consumer
```

```bash
npm --prefix src/client start
```
