---
name: campaign-info-editor
description: View or update the main README.md campaign information file for this D&D campaign repository. Use when asked to update general campaign info, campaign title, setting, DM name, system, house rules, themes, or campaign summary.
---

# Campaign info editor

Use this skill when asked to create, update, or refine the root `README.md` file of this D&D campaign repository.

## Location & Structure

The main campaign file is located at the root of the campaign repository:
`README.md`

It defines high-level, evergreen information about the campaign:
- **`# [Campagnetitel]`** — The first `# Heading` is used by The Wandering Wyvern as the official campaign title displayed in the header, navigation, and overview.
- **Metadata**:
  - `**Dungeon Master:** [DM Naam]`
  - `**Setting:** [Setting / Wereld]`
  - `**Systeem:** [Systeem & Editie, bijv. D&D 5e (2024)]`
- **`## Beschrijving`** — High-level campaign pitch / premise, world hook, and story summary.
- **`## Huisregels & Afspraken`** — Group agreements, table rules, character creation rules.
- **`## Thema's & Sfeer`** — Tonal guidelines, themes (e.g. gothic horror, heroic fantasy, political intrigue).

## Rules

- **Strictly evergreen:** describe the general campaign frame and premise, not session-by-session spoilers or live notes.
- **Maintain consistency:** verify that details align with existing `Lore/`, `NPCs/`, and `Locations/`.
- **Primary heading:** Always maintain the campaign title as the top `# Heading` so the viewer can parse it accurately.
