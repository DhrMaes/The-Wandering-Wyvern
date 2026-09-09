---
name: npc-creator
description: Create a new NPC file for this D&D campaign repo, following the folder structure, evergreen-content rule, and image conventions. Use when asked to add/create a new NPC or generic NPC archetype.
---

# NPC creator

Ask for anything not already given: the NPC's name, whether they're a named individual or a generic/reusable archetype, a short seed for appearance/personality/background, their **Class** (First/Second/Third), and their **job/title** if any (e.g. Bookshop owner, Prime Minister, Researcher, Guardian of Peace).

## Structure

- **Named NPC** (has an ongoing individual identity/relationships, e.g. Robert, Chumana): create a folder `NPCs/<kebab-case-name>/` containing:
  - `<kebab-case-name>.md` — the static character sheet: appearance, personality, background, secrets, ability scores/skills/stat block.
  - `logboek.md` — a DM-facing log recapping what actually happened with this NPC session by session, plus current status/relationship/open threads. Create it now with just a header (e.g. `# Logboek — <Name>`); it gets updated after every session the NPC appears in.
- **Generic/reusable archetype** (no individual identity, e.g. a generic guard statblock like `guardian-of-peace.md`): a single flat file `NPCs/<kebab-case-name>.md` — no folder, no `logboek.md`.

**CRITICAL METADATA RULE:** The top of the description file `<kebab-case-name>.md` MUST include the following fields directly below the main heading:
`**Type NPC:** [Named / Archetype]`

## Hard rule: always record Class and job/title

Right at the top of every NPC sheet (human NPCs; skip Class for dragons/non-human creatures where the class system doesn't apply — but still note a job/role if they have one), include a short "Basis" line or block stating:
- **Klasse**: First Class (rijk/machtig), Second Class (middenklasse), or Third Class (arm/onbevoorrecht) — per this campaign's class system.
- **Beroep/titel**: their job or title, if any (e.g. boekhandelaar, Eerste Minister, onderzoeker, Guardian of Peace). Omit this line only if the NPC genuinely has no job/title (e.g. a child, a generic prisoner).

This is settled, evergreen information — it belongs in the main sheet, not buried in background prose. It helps at the table to instantly gauge how an NPC fits into the class system and what leverage/knowledge their role might give them.

## Hard rule: keep the sheet strictly evergreen

`<kebab-case-name>.md` may only contain settled facts: appearance, personality, established background/relationships, current status as of the most recent session. **Never** add:
- forward-looking session plans or upcoming plot hooks
- "how to use this NPC next session" notes
- speculative behavior that hasn't happened yet

Any of that belongs in the relevant session's `outline.md`/`script.md` instead. This was a real pain point at the table — the character sheet must be safe to open mid-session without spoiling or cluttering it with things that may never happen.

## Images

- Optional full portrait, named after the NPC, inside the folder (e.g. `NPCs/robert/robert.png`) — roughly 16:9, meant to be printed and glued on cardboard for the table. Rendered in the entity modal.
- Optional square icon, same base name plus `.icon` (e.g. `NPCs/robert/robert.icon.png`) — used for the compact card grid in the Campaign Viewer. Not required; falls back to the full portrait, then a placeholder.
- For a flat-file generic archetype, the image (if any) goes next to the `.md` file with the same base name, same `.icon` convention.

### Generating the portrait with an image-generating AI agent

When creating a new NPC (or asked to add image generation for an existing one), also create a sibling file `NPCs/<kebab-case-name>/<kebab-case-name>.image-prompt.md` (next to the flat file for generic archetypes, e.g. `NPCs/<kebab-case-name>.image-prompt.md`) containing a ready-to-paste prompt for an image-generating AI agent. Keep this prompt in its own file rather than inside the main sheet, so the evergreen character sheet stays clean and uncluttered.

The prompt must specify these exact specs so the result is table-ready without manual cropping:

- **Total canvas: 3:4 aspect ratio (e.g., 1086 × 1448 px)** (portrait orientation), composed of two parts that must feel like **one unified illustration**, not two separate images glued together:
  - **Portrait section (Top ~85%)** — the actual NPC visual/portrait: the character in a fitting pose, outfit, and setting reflecting their appearance, personality, Class, and job/title. Keep the important visual focus (face/upper body) within the upper area so it doesn't spill into the card area.
  - **Nameplate section (Bottom ~15%)** — a visually integrated nameplate/card band. The background of this plaque must be textured appropriately (e.g., weathered parchment, classic woodwork, or rusted metal based on the context). It must clearly display, in-world/thematic styling appropriate to the campaign's tone:
    - The NPC's **name**
    - Their **job/title** (if any)
    - Their **Class** (First/Second/Third)
- **Framing (CRUCIAL)**: Explicitly instruct the AI that the *entire* image (both portrait and nameplate) must be framed by a thin, ornate, and detailed border with elegant corner flourishes. Do not hard-code the color (e.g., brass, gold, iron, silver) but let it fit the context and character.
- **Emblem (CRUCIAL)**: Instruct the AI that exactly in the middle of the bottom ornate border of the nameplate sits a small, detailed metallic emblem of a dragon.
- **Style**: Mandate "Hyper-realistic fantasy, photorealistic 3D/CGI, highly detailed textures, very high-quality cinematic lighting." Emphasize that it should not be cartoonish.
- Mention this is for a printable D&D handout meant to be printed and glued to cardboard for table use, plus reused as a cropped icon in a digital card grid — so it should be clean, legible at a glance, and not overly cluttered.
- Once an image is actually generated, save the full result as `NPCs/<kebab-case-name>/<kebab-case-name>.png` (or next to the flat file for generic archetypes). If a square icon is also wanted, crop the top portion and save it as `NPCs/<kebab-case-name>/<kebab-case-name>.icon.png`.

## Always

- Kebab-case file/folder names.
- One NPC per file.
- Cross-reference related entries with relative Markdown links (or inline `` `NPCs/x/x.md` `` code-path references — both render as clickable popups in the Campaign Viewer).
- Write the actual content in whichever language this campaign's `copilot-instructions.md` specifies.
- Check `Lore/` and existing NPCs/Locations before inventing names/dates/facts that might conflict with established canon.
