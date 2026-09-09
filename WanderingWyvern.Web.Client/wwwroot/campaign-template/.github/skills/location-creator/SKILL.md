---
name: location-creator
description: Create a new Location file/folder for this D&D campaign repo, following the folder structure, evergreen-content rule, and image conventions. Use when asked to add/create a new location, city, dungeon, region, or landmark.
---

# Location creator

Ask for anything not already given: the location's name and type (city, dungeon, region, landmark, building, ...), and a short seed for geography/atmosphere/inhabitants.

## Structure

Create a folder `Locations/<kebab-case-name>/` containing:
- A description file (e.g. `<kebab-case-name>.md`) — geography, atmosphere, notable inhabitants/features.
- Any associated map files/images or floor-plan notes (e.g. `floor-plan.md`, `map.png`) kept together in the same folder.

**CRITICAL METADATA RULE:** The top of the description file `<kebab-case-name>.md` MUST include the following fields directly below the main heading:
`**Type Locatie:** [type]`
`**Regio / Bovenliggende Locatie:** [regio of parent]`

## Hard rule: keep the description strictly evergreen

The description file covers only settled, evergreen facts — geography, atmosphere, established inhabitants. **Never** add:
- what happens there in a specific session
- forward-looking plot hooks tied to a particular visit
- session-specific NPC behavior at this location

That belongs in the relevant session's `outline.md`/`script.md` instead, linking back to the location. This avoids having to flip between the location file and the session file mid-table.

## Images

- Optional full map/scene image, named after the location, inside the folder (e.g. `Locations/featherswallow-house/map.png`) — roughly 16:9, print-ready. Rendered in the entity modal.
- Optional square icon, same base name plus `.icon` (e.g. `map.icon.png`) — used for the compact card grid. Not required; falls back to the full image, then a placeholder.

## Always

- Kebab-case folder/file names.
- One location per folder.
- Cross-reference related entries with relative Markdown links (or inline `` `Locations/x/x.md` `` code-path references — both render as clickable popups in the Campaign Viewer).
- Write the actual content in whichever language this campaign's `copilot-instructions.md` specifies.
- Check `Lore/` and existing Locations before inventing names/geography/facts that might conflict with established canon.
