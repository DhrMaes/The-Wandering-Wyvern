---
name: handout-creator
description: Create a new printable Handout file for this D&D campaign repo. Use when asked to add/create an in-world prop/letter or a player-facing rules cheat-sheet.
---

# Handout creator

Ask for anything not already given: what kind of handout this is (in-world prop/letter vs. rules cheat-sheet) and its topic.

## Structure

Create a folder `Handouts/<kebab-case-name>/` containing:
- A description file (e.g. `<kebab-case-name>.md`) containing the transcription/description of the handout.
- An image file of the handout (e.g. `handout.png`), if generated.

**CRITICAL METADATA RULE:** The top of the description file `<kebab-case-name>.md` MUST include the following fields directly below the main heading:
`**Type Handout:** [type]`

## Language depends on the handout's purpose

- **In-world props/letters** (things a player would receive in-fiction, e.g. a crumpled note, a court summons): write in the campaign's content language (see this campaign's `copilot-instructions.md`) — this is campaign content.
- **Rules cheat-sheets / class quick-references** (meta/technical, not campaign content): write in **English**.

## Always

- Kebab-case file names.
- Cross-reference related entries with relative Markdown links (or inline `` `Handouts/x.md` `` code-path references — both render as clickable popups in the Campaign Viewer).
- If the handout references an in-fiction item/NPC/location, link to its proper entity file rather than duplicating details.
