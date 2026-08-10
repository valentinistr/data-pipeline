# Data Pipeline

Angular 20 + .NET 10 monorepo for live coding interviews. Opens in GitHub Codespaces with the Dev Container tooling preconfigured.

## How to use this repo:

- Log into your GitHub account and navigate to the repository root. Make sure the `main` branch is selected.
- Click the `<> Code` button and select `Codespaces`.
- Click `Create codespace on main` and then open your newly created development environment.

## App design

![App design diagram of the Angular frontend, .NET controllers, in-memory database, event bus, and background import processor](app-design.png)

## Structure

- [`scripts/`](scripts/) — build / start / stop helpers
- [`src/backend/`](src/backend/) — .NET solution (`data-pipeline.sln`) with Api, Core, Database, and DataIngestion
- [`src/client/`](src/client/) — Angular 20 app (`ng serve`, port 4200)

## Tasks

**Terminal → Run Task…** offers four tasks, each backed by a script you can also run directly:

| Task | Script | What it does |
| --- | --- | --- |
| **Build** | `./scripts/build.sh` | `dotnet build` for Core, API, Database, DataIngestion, plus `ng build` (development) |
| **Start (hot reload)** | `./scripts/start.sh` | Stops what is running, builds, then starts the API and Angular with watching |
| **Start (no hot reload)** | `./scripts/start.sh --no-watch` | Same, but neither side watches for changes |
| **Stop** | `./scripts/stop.sh` | Stops the API, Angular, and their watchers |

**Ctrl+Shift+B** / **Cmd+Shift+B** runs **Build**, the default build task.

Starting builds the backend for you, so there is no need to run **Build** first. NuGet restore and
`npm ci` are skipped when their outputs are already current, so a normal start restores nothing:
restore runs only when a `.csproj`, the solution, or NuGet config is newer than
`obj/project.assets.json`, and `npm ci` runs only when `package-lock.json` is newer than the
installed tree. Angular is served by `ng serve`, so starting never runs a separate `ng build`.

Logs go to `.runtime/logs/`. Forwarded ports 5133 (API) and 4200 (client) appear in the Ports tab.

## Hot reload

The API runs under `dotnet watch`, so edits to method bodies apply without a restart. Changes
to `Program.cs`, DI registrations, or method signatures are rude edits and restart the process
automatically instead of prompting.

The Angular client runs `ng serve` with watch and live reload enabled, so edits under
`src/client/src` rebuild and refresh the browser. The forwarded 4200 URL serves the reload
socket over the same origin, so no extra Codespaces configuration is needed.

Pass `--no-watch` (or use the **Start (no hot reload)** task) to run a plain `dotnet run` and
serve the client without watching.

## Debugging

Hot reload and the debugger are independent, so attach only when you need breakpoints:

1. Start the app as usual.
2. **Run and Debug → Attach to API**.
3. In the process picker, choose `Api` (the one under `src/backend/Api/bin/Debug/`). `dotnet watch` starts the app as a grandchild, so attaching to a `dotnet watch` or `dotnet run` entry will not hit breakpoints.

Detaching (**Shift+F5**) leaves the app running. A rude edit restarts the process and ends the
debug session, so re-attach afterwards.

To debug startup code itself, use **Debug API (standalone)**, which launches the API under the
debugger. Stop the running API first, since it binds the same port.

## Running pieces manually

```bash
dotnet run --project src/backend/Api -c Debug
```

```bash
dotnet watch --project src/backend/Api --non-interactive -c Debug
```

```bash
npm --prefix src/client start
```
