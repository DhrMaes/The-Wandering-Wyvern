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
│       └── 🖼️ map.png
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

## Running it

### Locally (outside the container)

```bash
dotnet run --project WanderingWyvern.Web/WanderingWyvern.Web.csproj
```

Open the displayed URL and choose the campaign root folder in the browser.

### In the Dev Container

`WanderingWyvern.Web/bin` and `WanderingWyvern.Web/obj` are mounted as container-managed volumes to
avoid Windows bind-mount timestamp/permission issues with `dotnet watch`.

Then run the app in the container:

```bash
dotnet watch run --project /workspaces/WanderingWyvern/WanderingWyvern.Web
```

## Live notes

Each session page has a notes box. Anything typed there is appended, timestamped, to the selected
folder's `Sessions/<session-slug>/live-notes.md` through the browser's local file handle.

## Hosting

Users choose their own local campaign folder in the browser through the File System Access API;
campaign files are not uploaded to the server. Nginx Proxy Manager can provide HTTPS and routing,
while Authentik can protect the site. The application itself remains unaware of those deployment
concerns.

### Docker deployment

The production image serves The Wandering Wyvern WebAssembly application through ASP.NET Core on port `8080`.
Expose that port only to the Docker network used by Nginx Proxy Manager:

```bash
docker build -t wandering-wyvern .
docker run -d --name wandering-wyvern \
  --restart unless-stopped \
  -p 127.0.0.1:8080:8080 \
  wandering-wyvern
```

Configure Nginx Proxy Manager to forward your domain (e.g., `campaign.dhrmaes.com`) to the container, with Authentik protecting the Proxy Host and HTTPS terminated at Nginx Proxy Manager. No campaign directory volume is needed for the browser-local WebAssembly mode.

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
- A user must explicitly choose the campaign folder each time browser permissions are unavailable.
