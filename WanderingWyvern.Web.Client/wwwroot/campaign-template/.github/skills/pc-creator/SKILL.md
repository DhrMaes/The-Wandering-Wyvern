---
name: pc-creator
description: Create a new player character (PC) folder for this D&D campaign repo, following the folder structure and image conventions. Use when asked to add/create a new PC sheet or backstory.
---

# PC creator

Ask for anything not already given: the character's name, and a seed for their backstory/personality — this is player-authored content, so confirm details with the user rather than inventing their backstory.

## Structure

Create a folder `PCs/<kebab-case-name>/` containing:
- `<kebab-case-name>.md` — backstory, personality, established relationships, current status.
- Optionally a `logboek.md` — a DM-facing log of this PC's arc/status session by session, if useful to track separately from the recaps.

## Hard rule: keep the PC file strictly evergreen

`<kebab-case-name>.md` covers settled facts: backstory, personality, established relationships, current status. **Never** add session-specific forward-looking plans — that belongs in the relevant session's `outline.md`/`script.md`.

## Images

- Optional full portrait, named after the character, inside the folder (e.g. `PCs/princess-bibeth/princess-bibeth.jpg`) — roughly 16:9, print-ready. Rendered in the entity modal.
- Optional square icon, same base name plus `.icon` (e.g. `princess-bibeth.icon.jpg`) — used for the compact card grid. Not required; falls back to the full portrait, then a placeholder.

## Always

- Kebab-case folder/file names.
- One PC per folder.
- Cross-reference related entries with relative Markdown links (or inline `` `PCs/x/x.md` `` code-path references — both render as clickable popups in the Campaign Viewer).
- Write the actual content in whichever language this campaign's `copilot-instructions.md` specifies.
- Check `Lore/` and existing PCs/NPCs before inventing names/facts that might conflict with established canon.

