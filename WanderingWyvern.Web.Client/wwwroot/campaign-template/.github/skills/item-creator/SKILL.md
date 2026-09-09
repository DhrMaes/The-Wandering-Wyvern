---
name: item-creator
description: Create a new Item folder for this D&D campaign repo, following the folder structure and image conventions. Use when asked to add/create a new notable item, artifact, or magic item.
---

# Item creator

Ask for anything not already given: the item's name and a short seed for its description/history/mechanical effect.

## Structure

Create a folder `Items/<kebab-case-name>/` containing:
- A description file (e.g. `<kebab-case-name>.md`) explaining mechanics and lore.
- An image file of the item (e.g. `image.png`), if generated.

**CRITICAL METADATA RULE:** The top of the description file `<kebab-case-name>.md` MUST include the following fields directly below the main heading:
`**Type Item:** [type]`
`**Zeldzaamheid:** [rarity]`
- Optionally a `logboek.md` — a DM-facing log of what happened to/with this item session by session (only needed if the item's status/possession changes over time; skip it for simple static items).

## Hard rule: keep the item file strictly evergreen

`<kebab-case-name>.md` covers only settled facts: description, history, mechanical properties/effects. **Never** add session-specific plans (e.g. "the players will find this in session 4's vault") — that belongs in the relevant session's `outline.md`/`script.md`. Reference the item from there via a link or inline `` `Items/x/x.md` `` code-path instead of duplicating its details.

## Images

- Optional full image, named after the item, inside the folder (e.g. `Items/johns-notitieboekje/johns-notitieboekje.png`) — roughly 16:9, print-ready. Rendered in the entity modal.
- Optional square icon, same base name plus `.icon` (e.g. `johns-notitieboekje.icon.png`) — used for the compact card grid. Not required; falls back to the full image, then a placeholder.

## Always

- Kebab-case folder/file names.
- One item per folder.
- Cross-reference related entries with relative Markdown links (or inline `` `Items/x/x.md` `` code-path references — both render as clickable popups in the Campaign Viewer).
- Write the actual content in whichever language this campaign's `copilot-instructions.md` specifies.
- Check `Lore/` and existing Items before inventing names/history that might conflict with established canon.

