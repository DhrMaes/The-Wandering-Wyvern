# The Wandering Wyvern

A live, at-the-table D&D campaign workspace for Markdown files. It renders sessions as a split-pane
(outline + read-aloud script), lets you browse campaign entities, and lets you jot down timestamped
notes during play that get written straight into the session's `live-notes.md` for later use in the
recap.

The application runs in the browser and reads the campaign folder selected by the user through the
File System Access API. Campaign files are not uploaded to the server.

## Stack

- **ASP.NET Core Blazor Web App** (.NET 10) — hosts the WebAssembly application.
- **Blazor WebAssembly** — reads and indexes the selected local campaign folder in the browser.
- **Markdig** for Markdown → HTML rendering.
- **File System Access API** — reads local Markdown/images and writes live notes without uploads.
- **Dev Container** (`.devcontainer/`) so the environment (the .NET SDK version, etc.) is identical
  regardless of who runs it.

## Content convention this viewer expects

This viewer is generic — it doesn't hardcode anything about *this* campaign. It expects the
content repository it's pointed at to follow this folder layout (as used by, e.g., the
"A Language of Dragons Campaign" repo):

```text
📦 <campaign-root>
├── 📂 Sessions/
│   └── 📂 session-##-title/
│       ├── 📄 outline.md
│       ├── 📄 script.md
│       └── 📄 recap.md
├── 📂 NPCs/
│   ├── 📂 <name>/
│   │   ├── 📄 <name>.md
│   │   ├── 📄 logboek.md
│   │   ├── 🖼️ <name>.png (optional)
│   │   └── 🖼️ <name>.icon.png (optional)
│   └── 📄 <generic-archetype>.md
├── 📂 Locations/
│   └── 📂 <name>/
│       ├── 📄 description.md
│       ├── 📄 floor-plan.md
│       ├── 🖼️ <name>.png
│       └── 🗺️ <name>.map.png
├── 📂 Lore/
│   ├── 📂 <topic>/
│   │   └── 📄 <topic>.md
│   └── 📄 <flat-topic>.md
├── 📂 PCs/
│   ├── 📂 <name>/
│   │   ├── 📄 <name>.md
│   │   ├── 🖼️ <name>.png (optional)
│   │   └── 🖼️ <name>.icon.png (optional)
│   └── 📄 <flat-pc>.md
├── 📂 Items/
│   ├── 📂 <name>/
│   │   ├── 📄 <name>.md
│   │   ├── 🖼️ <name>.png (optional)
│   │   └── 🖼️ <name>.icon.png (optional)
│   └── 📄 <flat-item>.md
├── 📂 Handouts/
│   ├── 📂 <name>/
│   │   ├── 📄 <name>.md
│   │   ├── 🖼️ <name>.png (optional)
│   │   └── 🖼️ <name>.icon.png (optional)
│   └── 📄 <flat-handout>.md
└── 📂 References/    (unstructured content, linkable but not in nav)
```

A document's title in the UI is taken from its first `# Heading`, falling back to a humanized file
name. Relative Markdown links (`[Robert](../NPCs/robert/robert.md)`) are resolved against the
content root and, if they match an indexed document, become links to that document.

### Image convention

Any image sitting next to (or inside the folder of) an entity's Markdown file is picked up
automatically:

- **Full image** — named after the entity (e.g. `robert.png`, `princess-bibeth.jpg`, `map.png`).
  Typically a ~16:9 portrait/map, good for printing and gluing on cardboard for the table.
- **Icon image** *(optional)* — same base name plus `.icon` (e.g. `robert.icon.png`,
  `princess-bibeth.icon.jpg`). A square crop meant for compact card grids and session lists. When
  present, the card grid prefers the icon over the full image; when absent, it falls back to the
  full image, then to a placeholder glyph.
- **Location map** — any image in a location folder whose filename ends with `.map` before the
  extension (e.g. `docks.map.webp`). Maps appear in a gallery when opening that location and are
  excluded from the location card image selection.

## Running it

### Locally (outside the container)

```bash
dotnet run --project WanderingWyvern.Web/WanderingWyvern.Web.csproj
```

Open the displayed URL and choose the campaign root folder in the browser.

When using VS Code, press `F5` and select **Run The Wandering Wyvern**. The repository's
`.vscode` setup builds the Web host, starts it with the Development environment, and opens the
local URL automatically.

### Browser regression tests

The repository includes C# Playwright smoke tests for the responsive navbar and loading overlay.
Install the Chromium browser once after building the solution. The tests use MSTest and can be
executed through either `dotnet test` or VSTest:

```powershell
dotnet build WanderingWyvern.slnx
pwsh .\WanderingWyvern.Web.Tests\bin\Debug\net10.0\playwright.ps1 install chromium
dotnet test WanderingWyvern.Web.Tests\WanderingWyvern.Web.Tests.csproj
# Or, after building:
dotnet vstest WanderingWyvern.Web.Tests\bin\Debug\net10.0\WanderingWyvern.Web.Tests.dll
```

### In the Dev Container

All project `bin` and `obj` directories are mounted as container-managed volumes to avoid Windows
bind-mount timestamp/permission issues with `dotnet watch` and solution-level builds.

Then run the app in the container:

```bash
dotnet watch run --project /workspaces/WanderingWyvern.Web
```

The dev container's `postCreateCommand` restores and builds the browser-test project, then force
installs Playwright Chromium, including its headless shell and Linux dependencies. This also repairs
an incomplete browser cache when the container is recreated. Run the browser regressions with:

```bash
dotnet test /workspaces/WanderingWyvern.Web.Tests/WanderingWyvern.Web.Tests.csproj
```

After changing the devcontainer configuration, use **Dev Containers: Rebuild Container** so the
Linux build-output volumes are created.

## Live notes

Each session page has a notes box. Anything typed there is appended, timestamped, to the selected
folder's `Sessions/<session-slug>/live-notes.md` through the browser's local file handle.

## Hosting

Users choose their own local campaign folder in the browser through the File System Access API;
campaign files are not uploaded to the server. Nginx Proxy Manager can provide HTTPS and routing,
while Authentik can protect the site. The application itself remains unaware of those deployment
concerns.

### Docker deployment

The production image serves The Wandering Wyvern WebAssembly application through ASP.NET Core on port `8090`.
The repository includes [`docker-compose.yml`](docker-compose.yml) for Portainer and other Compose-compatible
deployments:

```yaml
services:
  wandering-wyvern:
    image: ghcr.io/dhrmaes/wandering-wyvern:${IMAGE_TAG:-latest}
```

In Portainer, create a Git-based Stack pointing to this repository and use `docker-compose.yml` as the Compose file path.
Set the stack environment variable `IMAGE_TAG` to the published image tag, or leave it unset to use `latest`.
No campaign directory volume is needed for browser-local WebAssembly mode.

Configure Nginx Proxy Manager to forward your domain (e.g., `campaign.dhrmaes.com`) to
the Docker host on port `8090`, with Authentik protecting the Proxy Host and HTTPS terminated at
Nginx Proxy Manager. If Nginx Proxy Manager runs in another container, use the Docker host's
reachable IP address rather than `127.0.0.1`.

The GitHub Actions workflow in `.github/workflows/docker.yml` builds the image on git tag pushes, publishes `latest` and version tags to GHCR, then connects to WireGuard and redeploys the Portainer stack using the tag name.
The stack should reference the immutable tag through an environment variable:

```yaml
services:
  wandering-wyvern:
    image: ghcr.io/<github-owner>/wandering-wyvern:${IMAGE_TAG:-latest}
```

Configure these repository secrets for automatic deployment: `WG_CONFIG`, `PORTAINER_URL`,
`PORTAINER_API_KEY`, `PORTAINER_STACK_ID`, and `PORTAINER_ENDPOINT_ID`. The Docker host must also
be able to pull the private GHCR image.

## Known limitations

- The File System Access API works best in Chrome and Edge on desktop.
- The app remembers the last campaign folder when the browser preserves its directory handle and
  permission; otherwise the user must choose the folder again.
