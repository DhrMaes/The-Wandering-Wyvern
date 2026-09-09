---
name: lore-creator
description: Create a new Lore folder for this D&D campaign repo, following the folder structure. Use when asked to add/create world history, factions, religions, languages, timelines, or other background lore.
---

# Lore creator

Ask for anything not already given: the lore topic and a short seed for its content.

## Structure

Create a folder `Lore/<kebab-case-name>/` containing:
- A description file (e.g. `<kebab-case-name>.md`) detailing the lore.
- Any related imagery or texts kept alongside it.

**CRITICAL METADATA RULE:** The top of the description file `<kebab-case-name>.md` MUST include the following fields directly below the main heading:
`**Type Lore:** [type]`

## Hard rule: keep it strictly evergreen and consistent

Lore entries are the source of truth for world history/factions/religions/languages/timelines. **Before writing**, check existing `Lore/` files and referenced `NPCs/`/`Locations/` for names, dates, or facts that might conflict with established canon — lore is the thing everything else has to stay consistent with. Never add session-specific content here; lore describes the world, not what happens to the party in it.

## Always

- Kebab-case folder/file names.
- One lore topic per folder.
- Cross-reference related entries with relative Markdown links (or inline `` `Lore/x/x.md` `` code-path references — both render as clickable popups in the Campaign Viewer).
- Write the actual content in whichever language this campaign's `copilot-instructions.md` specifies.

