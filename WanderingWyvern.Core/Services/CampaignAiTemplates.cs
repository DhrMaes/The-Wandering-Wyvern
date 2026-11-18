namespace DhrMaes.WanderingWyvern.Core.Services;

/// <summary>
/// Built-in AI skills, agents, and configuration templates that are synced to opened campaign folders.
/// </summary>
public static class CampaignAiTemplates
{
    public static readonly IReadOnlyDictionary<string, string> Files = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        [".agents/skills.json"] = @"{
  ""entries"": [
    {
      ""path"": "".github/skills""
    },
    {
      ""path"": "".github/agents""
    }
  ]
}
",
        [".github/skills/AGENTS.md"] = @"**CRITICAL RULE:**
At the start of any task, you MUST always ask the user if he want to use the campaign master persona. If yes, you MUST use your file tools to read `.github/agents/campaign-master.agent.md` and adopt that persona before proceeding. If no, you MUST use your default persona.
",
        [".github/agents/campaign-master.agent.md"] = @"---
name: campaign-master
description: Master Dungeon Master co-pilot for The Wandering Wyvern campaign manager. Expert in world-building, 5e mechanics, branching session design with Mermaid flowcharts, and maintaining evergreen campaign canon.
---

# Campaign Master

You are the Campaign Master, an expert D&D 5e Dungeon Master assistant and world-building architect specifically tailored for **The Wandering Wyvern** campaign repository system.

## Core Philosophy

1. **Strict 4-File Session Architecture:** Every session folder under `Sessions/session-##-title/` contains strictly 4 files:
   - `outline.md` — Lean regie overview, goals, encounter summaries, and entity links.
   - `script.md` — The DM's verbatim table script with read-aloud text (`> blockquote`), roll gradients (DC tiers), and the **Mermaid flowchart** for interactive branching.
   - `live-notes.md` — Table notebook with live timestamps and auto-logged branch decisions.
   - `recap.md` — Post-session narrative summary of what actually happened along the chosen path.

2. **Branching & Dynamic Flowcharts:**
   - Always structure multi-path sessions with a `flowchart TD` Mermaid block at the top of `script.md`.
   - Use descriptive choice labels on edges (`-->|Keuze|`) and matching scene headings (`## Scène 1 — Titel`).
   - The Wandering Wyvern utilizes this graph for interactive table buttons, unchosen branch collapsing, auto-logging to `live-notes.md`, and the visual Flow Canvas builder.

3. **Evergreen Entity Principle:**
   - `NPCs/`, `Locations/`, `Items/`, `Lore/`, and `PCs/` contain only settled, established facts.
   - Never place forward-looking session prep in entity files.
   - Every named NPC has a static sheet (`<name>.md`) and a session history log (`logboek.md`).

4. **Clickable Hyperlinks:**
   - Link all referenced entities with relative markdown links (e.g., `[Robert](NPCs/robert/robert.md)`) or inline code paths (`` `NPCs/robert/robert.md` ``), which render as interactive modal popups in The Wandering Wyvern.

5. **Language & Tone:**
   - Write campaign content in the language specified by the campaign's `copilot-instructions.md` (defaults to Dutch/Flemish if specified, or the user's preferred language).
",
        [".github/skills/campaign-info-editor/SKILL.md"] = @"---
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
",
        [".github/skills/encounter-builder/SKILL.md"] = @"---
name: encounter-builder
description: Design a mathematically balanced combat encounter with 5e CR math, stat blocks, and tactical suggestions.
---

# Encounter Builder

Use this skill when asked to create or balance a combat encounter. Ask for the **Party Level**, **Party Size** (number of PCs), and the **Desired Difficulty** (Easy, Medium, Hard, or Deadly) if not provided.

## 1. Balance the Encounter
- Use standard D&D 5e encounter building rules (XP thresholds) to build the encounter.
- Calculate the party's XP threshold for the desired difficulty.
- Select or design monsters that fit the thematic setting, ensuring their combined adjusted XP matches the target difficulty.

## 2. Stat Blocks
- Provide complete, correct 5e stat blocks for any custom or modified enemies.
- Ensure all basic stats are included: AC, HP, Speed, Ability Scores, Saving Throws, Skills, Senses, Languages, Challenge Rating.
- Detail their Actions, Bonus Actions, Reactions, and any special traits.

## 3. Environment & Tactics
- Suggest environmental features that make the encounter interesting (e.g., difficult terrain, cover, hazards, lighting).
- Provide a brief ""Tactics"" section explaining how the enemies will fight (e.g., who they target, when they flee, how they use the environment).

## Output
Format the output cleanly in Markdown, using tables for stat blocks or clear bolded lists so the DM can easily read it during play. If this encounter belongs in a session, output it directly into the relevant `script.md` or `outline.md` file.
",
        [".github/skills/handout-creator/SKILL.md"] = @"---
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
",
        [".github/skills/item-creator/SKILL.md"] = @"---
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

`<kebab-case-name>.md` covers only settled facts: description, history, mechanical properties/effects. **Never** add session-specific plans (e.g. ""the players will find this in session 4's vault"") — that belongs in the relevant session's `outline.md`/`script.md`. Reference the item from there via a link or inline `` `Items/x/x.md` `` code-path instead of duplicating its details.

## Images

- Optional full image, named after the item, inside the folder (e.g. `Items/johns-notitieboekje/johns-notitieboekje.png`) — roughly 16:9, print-ready. Rendered in the entity modal.
- Optional square icon, same base name plus `.icon` (e.g. `johns-notitieboekje.icon.png`) — used for the compact card grid. Not required; falls back to the full image, then a placeholder.

## Always

- Kebab-case folder/file names.
- One item per folder.
- Cross-reference related entries with relative Markdown links (or inline `` `Items/x/x.md` `` code-path references — both render as clickable popups in the Campaign Viewer).
- Write the actual content in whichever language this campaign's `copilot-instructions.md` specifies.
- Check `Lore/` and existing Items before inventing names/history that might conflict with established canon.

",
        [".github/skills/live-notes-to-recap/SKILL.md"] = @"---
name: live-notes-to-recap
description: Turn a session's live-notes.md (raw notes and auto-logged branch choices jotted during play via the Campaign Viewer) into a proper recap.md, and update any touched NPC/Location logboek files. Use when asked to write up, finish, or wrap up a session's recap.
---

# Live notes → recap

## Steps

1. Read `Sessions/session-##-title/live-notes.md` (the timestamped notes jotted during play through the Campaign Viewer's live-notes panel, including automated branch choices like `[HH:mm] Keuze: ... (Scène: ...)`), plus `outline.md` and `script.md` for context on what was actually planned vs. what happened.
2. Trace the **actual chosen story path**: Check the logged choices in `live-notes.md` against the Mermaid flowchart in `script.md` to identify the route taken by the party, noting which branching paths were selected and which scenes were bypassed.
3. Ask the user to fill in or confirm anything ambiguous (dice-roll outcomes, player decisions not captured in the raw notes) rather than guessing or inventing what happened.
4. Write `Sessions/session-##-title/recap.md` in narrative/in-fiction prose (not a bullet log), in whichever language this campaign's `copilot-instructions.md` specifies — a readable summary suitable for refreshing players' memories before the next session. Emphasize the events and choices that actually unfolded along the party's path.
5. Update every NPC's/Location's `logboek.md` that was touched this session with the new status/relationship/open threads — settled facts only, not speculation about what's coming next.
6. If new NPCs/Locations/Items were introduced ad hoc during play (not pre-planned in `outline.md`), create proper stub entities for them now, rather than leaving them undocumented.
7. Leave `live-notes.md` as-is afterwards — it's the historical raw record, don't delete or overwrite it.
",
        [".github/skills/location-creator/SKILL.md"] = @"---
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
",
        [".github/skills/loot-generator/SKILL.md"] = @"---
name: loot-generator
description: Generate treasure hoards, magic items, or shop inventories using standard D&D 5e economy rules.
---

# Loot Generator

Use this skill when asked to create loot for a monster, dungeon, or a shop's inventory. Ask for the **Challenge Rating (CR)** of the encounter or the **theme/wealth level** of the shop if not provided.

## 1. Treasure Hoards
- For boss fights or major stashes, generate a treasure hoard based on the encounter's CR tier (0-4, 5-10, 11-16, 17+).
- Include standard coinage (CP, SP, EP, GP, PP), art objects/gems, and potential magic items.
- Ensure the values are mathematically sound according to 5e rules.

## 2. Magic Items
- When suggesting magic items, stick to standard 5e rarities (Common, Uncommon, Rare, Very Rare, Legendary).
- If inventing a custom item, provide a clear mechanical description (Attunement requirements, charges, specific effects) that aligns with 5e balance.

## 3. Shop Inventories
- If generating a shop, provide a list of available items with their standard Player's Handbook (PHB) prices.
- If the shop is in an isolated or affluent area, adjust prices narratively (e.g., 1.5x cost) and note the markup.

## Output
Format the loot clearly, using bullet points for individual items and bold text for total gold values. If generating a shop, use a Markdown table with columns for Item, Description, and Cost.
",
        [".github/skills/lore-creator/SKILL.md"] = @"---
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

",
        [".github/skills/npc-creator/SKILL.md"] = @"---
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

Right at the top of every NPC sheet (human NPCs; skip Class for dragons/non-human creatures where the class system doesn't apply — but still note a job/role if they have one), include a short ""Basis"" line or block stating:
- **Klasse**: First Class (rijk/machtig), Second Class (middenklasse), or Third Class (arm/onbevoorrecht) — per this campaign's class system.
- **Beroep/titel**: their job or title, if any (e.g. boekhandelaar, Eerste Minister, onderzoeker, Guardian of Peace). Omit this line only if the NPC genuinely has no job/title (e.g. a child, a generic prisoner).

This is settled, evergreen information — it belongs in the main sheet, not buried in background prose. It helps at the table to instantly gauge how an NPC fits into the class system and what leverage/knowledge their role might give them.

## Hard rule: keep the sheet strictly evergreen

`<kebab-case-name>.md` may only contain settled facts: appearance, personality, established background/relationships, current status as of the most recent session. **Never** add:
- forward-looking session plans or upcoming plot hooks
- ""how to use this NPC next session"" notes
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
- **Style**: Mandate ""Hyper-realistic fantasy, photorealistic 3D/CGI, highly detailed textures, very high-quality cinematic lighting."" Emphasize that it should not be cartoonish.
- Mention this is for a printable D&D handout meant to be printed and glued to cardboard for table use, plus reused as a cropped icon in a digital card grid — so it should be clean, legible at a glance, and not overly cluttered.
- Once an image is actually generated, save the full result as `NPCs/<kebab-case-name>/<kebab-case-name>.png` (or next to the flat file for generic archetypes). If a square icon is also wanted, crop the top portion and save it as `NPCs/<kebab-case-name>/<kebab-case-name>.icon.png`.

## Always

- Kebab-case file/folder names.
- One NPC per file.
- Cross-reference related entries with relative Markdown links (or inline `` `NPCs/x/x.md` `` code-path references — both render as clickable popups in the Campaign Viewer).
- Write the actual content in whichever language this campaign's `copilot-instructions.md` specifies.
- Check `Lore/` and existing NPCs/Locations before inventing names/dates/facts that might conflict with established canon.
",
        [".github/skills/pc-creator/SKILL.md"] = @"---
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

",
        [".github/skills/session-prep/SKILL.md"] = @"---
name: session-prep
description: Prep a new D&D campaign session (outline.md + script.md) with interactive branching and Mermaid flowcharts for this repo. Use when asked to prep, plan, or start prepping a new session, or to add scenes/content to an existing session's outline or script.
---

# Session prep

Prep a session under `Sessions/session-##-title/`, following the lessons below (learned from actually running sessions at the table).

## What goes where

- **`outline.md`** — DM-facing regie notes: scene structure, decision points, mechanical prep (encounters, DCs, loot), pacing, and a ""Benodigde NPCs/Locaties/Items"" links section. Start with a `**Datum:** YYYY-MM-DD` metadata line.
- **`script.md`** — everything the DM actually needs to *run* the session out loud, including the **Mermaid flowchart** for interactive branching. Build it side by side with `outline.md` from the start — never bolt it on later.

## `outline.md` structure — keep it short and to the point

`outline.md` is a quick-glance regie reference, **not** a second script. Follow this shape:

1. `# Sessie <nummer> — <Titel>` (Example: `# Sessie 4 — Echolocation`)
2. `**Sessie:** <nummer>`
3. `**Datum:** YYYY-MM-DD` — or `**Datum:** TBD` if not scheduled yet.
4. `**In-Game Datum:** [in-game datum]`
5. `## Doel van de sessie` — one or two sentences on what this session needs to accomplish (the emotional/plot beat, not a scene-by-scene recap).
6. One short section per scene, each just a **quick, decisive glance**: what happens, key checks/DCs, and links to the NPCs/locations/items involved. A sentence or two per scene is usually enough — the *how to say it* content belongs in `script.md`, not here. Don't restate read-aloud text or write out full dialogue in the outline.
7. A **""Benodigde NPCs/Locaties/Items""** section linking everything needed for the session.

Avoid the anti-pattern seen in `Sessions/session-02-the-caged-dragon/outline.md`, which grew into a mix of outline *and* script (full DC tables, extended prose, near-duplicate scene descriptions) — that made it a second script.md instead of a quick reference. Keep new outlines lean; put all the atmosphere, exact wording, and branching detail in `script.md` instead.

## `script.md` structure — the DM's actual table tool

1. `# Sessie ## — Voorleesscript voor de DM`
2. `**Datum:** YYYY-MM-DD` (or `TBD`) plus a short italic companion note pointing back to `outline.md`.
3. **Mermaid Flowchart for Branching & Flow Builder (CRITICAL):**
   Directly below the metadata, include a `mermaid` code block (`flowchart TD` or `graph TD`) defining the session's scenes and decision branches. The Wandering Wyvern parses this block to power the interactive Flow Canvas, table-side Choice Bar, dynamic branch collapsing, and live choice logging:
   ```mermaid
   flowchart TD
       S1[""Scène 1 — Het Kruispunt""] -->|Kies het linkerpad| S2[""Scène 2 — Het Moeras""]
       S1 -->|Kies het rechterpad| S3[""Scène 3 — De Verlaten Wachttoren""]
       S2 --> S4[""Scène 4 — De Oude Ruïne""]
       S3 --> S4
   ```
   - **Node IDs:** Use clean IDs like `S1`, `S2`, `S3` or `1`, `2`, `3`.
   - **Edge labels:** Put descriptive choice or trigger text on arrows: `-->|Keuzetekst|`.
   - **Convergence:** Different paths can converge into shared scenes or branch into separate climaxes.
4. One `## Scène N — Titel` section per scene (matching the IDs and titles from the flowchart), in logical play order, each containing:
   - The **literal read-aloud text**, wrapped in `> blockquote`, ready to say almost verbatim.
   - Regie notes around it: what to do, what checks to call for, and how to react to results.
   - **Branch on roll results explicitly, don't just gate pass/fail.** For any check where the outcome should scale (e.g. searching a room, gathering information, persuading someone), write out what happens at meaningful DC tiers, not just success/failure — for example:
     > *Investigation-check om de kamer te doorzoeken:*
     > - **DC 10 (bv. een 7 totaal):** ze merken de voor de hand liggende dingen op — een omgekeerde stoel, een opengebroken kast.
     > - **DC 14:** ze vinden bovendien een verborgen laatje met een half verbrande brief.
     > - **DC 18+:** ze zien ook het kleine detail dat alles verraadt — het slot is van binnenuit geforceerd, niet van buiten.

     This is the place to think ahead and be creative: sketch out a few tiers of ""what they notice/learn"" ahead of time so you're not improvising the gradient live at the table.
   - Optional side-checks/side-content clearly marked **""Optioneel:""**, wrapped in a collapsible `<details><summary>Optioneel: ...</summary> ... </details>` block (rendered natively by the Campaign Viewer) so the script stays uncluttered at the table and can be expanded only if that thread comes up, e.g.:
     > ```html
     > <details>
     > <summary>Optioneel: Oswalt vraagt om verontschuldiging</summary>
     >
     > *(volledige uitwerking van dit spoor)*
     > </details>
     > ```

## Interactive Branching & Table Experience

The Wandering Wyvern automatically interprets the Mermaid diagram in `script.md`:
- **Dynamic Choice Bar:** Choices on outgoing branches appear as interactive buttons at the top of the session view.
- **Auto Branch Collapsing:** When the DM selects a path, irrelevant scenes from unchosen branches are collapsed into clean peek cards (`[ ◷ Niet gekozen (klik om in te kijken) ]`), keeping the DM focused while allowing them to peek if players backtrack.
- **Auto Live Notes:** Making a choice automatically records a timestamped entry in `live-notes.md` (e.g. `[14:32] Keuze: Kies het linkerpad (Scène: Scène 2 — Het Moeras)`).
- **Interactive Flow & Builder:** DMs can view and edit the entire session flow visually in the `☍ Flow & Builder` tab, connecting nodes, dragging arrows to empty canvas to create new scenes, and moving scenes around.

## Hard rules learned from actually running sessions

1. **`script.md` needs the exact wording, not a summary.** When a scene, room, or item needs to be described to the players, write the literal text the DM should read/paraphrase aloud — full sentences, in the campaign's content language, ready to use at the table. Don't just note ""describe the room"" — write the description itself. Wrap read-aloud text in `> blockquote` (the Campaign Viewer renders these specially, so read-aloud text is visually obvious mid-session).
2. **Every flowchart node must have a corresponding scene header.** Ensure each node in the Mermaid block corresponds to a `## Scène ...` header so the viewer can link navigation and choice buttons directly to the content.
3. **No clutter, no dead information.** Only include what's actually needed to run *this* session. Do **not** add notes like ""Rita is not present this session"" or other negative/non-actionable trivia — if an NPC/location/item isn't part of the session, simply don't mention it. Every line in `script.md` should be something the DM will actually say or use.
4. **Zero session-specific content lives in the entity files.** NPCs/Locations/Items/Lore/PCs must stay strictly evergreen. If you need session-specific behavior for an existing NPC/Location, put it directly in `outline.md`/`script.md` — never a separate file in the session folder (see below), and never in the NPC's/Location's own file. This was a real pain point: flipping through character files mid-session for stuff that should've been in the session prep.
5. **Reference entities by link, don't duplicate them.** Now that Markdown links and inline `` `Path/file.md` `` code-spans both render as clickable modal popups in the Campaign Viewer, use those instead of copy-pasting an NPC's/location's/item's details into the session file. Link out; only inline the parts that are genuinely session-specific (like ""today, Robert also mentions..."").
6. Keep expanding the ""Benodigde NPCs/Locaties/Items"" list in `outline.md` as prep continues — don't leave it to patch in later, and don't leave it to the session itself.
7. For every NPC/Location/Item referenced that doesn't exist yet, create a stub now (matching the entity's own folder/file conventions) rather than leaving a dangling reference.
8. Write all actual campaign content in whichever language this campaign's `copilot-instructions.md` specifies.

Do not create `recap.md` yet — that's written after the session.

## Folder/file naming

`Sessions/session-##-title/` — kebab-case, zero-padded two-digit session number (e.g. `session-03-the-broken-seal`).

## A session folder always has exactly these 4 files

`Sessions/session-##-title/` contains **only**: `outline.md`, `script.md`, `live-notes.md` (created empty/ready before the session, filled live during play via the Campaign Viewer), and `recap.md` (written after the session). Do not add extra session-scoped files (e.g. a separate `<npc-slug>.md` for one NPC's session-specific behavior) — that content belongs directly inside `outline.md`/`script.md` instead.
",
        [".github/skills/skill-challenge-creator/SKILL.md"] = @"---
name: skill-challenge-creator
description: Design a structured, non-combat encounter (like a chase or negotiation) using 4e-style skill challenge mechanics.
---

# Skill Challenge Creator

Use this skill when the DM needs to formalize a tense, multi-step, non-combat scenario (e.g., a rooftop chase, navigating a dangerous storm, securing a treaty).

## 1. Goal and Complexity
- Define the ultimate **Goal** of the challenge.
- Set the complexity: typically **""X Successes before 3 Failures""** (e.g., 3 successes for a quick hurdle, 5 for a standard challenge, 7 for an epic ordeal).

## 2. Skills and DCs
- Suggest appropriate Base DCs based on the party level (e.g., DC 10-15 for low level, DC 15-20 for mid level).
- Define **Primary Skills**: 3-5 skills that directly advance the goal (e.g., Athletics to run, Acrobatics to dodge). Each successful check grants 1 Success.
- Define **Secondary Skills**: Skills that don't grant a Success but provide a benefit (e.g., granting Advantage on the next Primary check, or reducing the DC).

## 3. Complications and Consequences
- Briefly describe what happens on a **Success** (the party achieves the goal).
- Describe what happens on a **Failure** (they fail the goal, or they succeed but with a major consequence like lost hit dice, exhaustion, or a new enemy).
- Suggest a complication that might occur mid-challenge (e.g., the weather worsens, raising the DC by 2).

## Output
Format the challenge clearly in Markdown. If this belongs in a session, wrap it in a `<details><summary>Skill Challenge: [Name]</summary>` block so it doesn't clutter the main script unless needed.
",
        [".github/skills/SKILL.md"] = @"---
name: downtime-manager
description: Resolve between-session downtime activities like crafting, carousing, or researching lore using standard 5e rules.
---

# Downtime Manager

Use this skill when the DM asks to resolve downtime activities for characters between adventures. Ask for the **Activity**, the **Character Level**, and the **Time Spent** (in days/weeks) if not provided.

## 1. Standard Downtime Rules
- Apply standard 5e downtime rules (from the DMG or Xanathar's Guide) for activities like:
  - **Crafting** (mundane or magic items, potions)
  - **Carousing** (making contacts, gathering rumors)
  - **Research** (uncovering lore, translating texts)
  - **Working/Running a Business**
  - **Training** (learning a language or tool proficiency)

## 2. Resolution and Costs
- Define the gold or resource cost required to start the activity.
- Determine what ability checks (if any) are required to resolve the activity and provide the DCs.
- Calculate the outcome based on the time spent and the check results.

## 3. Complications
- Roll or suggest a 10% chance of a complication arising from the downtime activity (e.g., a rival interferes, the crafted item is flawed, the research attracts unwanted attention).

## Output
Provide a clear, step-by-step resolution for the DM to read or hand to the player. Use Markdown lists and bold key mechanics (Gold, Time, DCs).
",
        [".github/skills/trap-and-puzzle-creator/SKILL.md"] = @"---
name: trap-and-puzzle-creator
description: Generate engaging traps and puzzles with proper triggers, effects, and countermeasures (DCs).
---

# Trap and Puzzle Creator

Use this skill when asked to create a trap or puzzle for a dungeon, hideout, or encounter. Ask for the **Party Level** and the **Theme/Location** (e.g., ancient tomb, thieves' guild, wizard tower) if not provided.

## 1. Description and Trigger
- Describe what the trap/puzzle looks like to the players initially.
- Clearly define the **Trigger**: exactly what action causes the trap to spring (e.g., stepping on a specific tile, opening a chest without the key).

## 2. Effects
- Detail what happens when the trap triggers.
- Provide the required **Saving Throw** and the **Base DC** (appropriate for the party level).
- Specify the damage (using standard 5e damage scaling by tier) and any conditions applied (e.g., Poisoned, Restrained).

## 3. Countermeasures (DCs)
- **Notice/Spot:** The Passive Perception or active Wisdom (Perception) / Intelligence (Investigation) DC required to spot the trap before it triggers.
- **Disarm/Bypass:** The Dexterity (Thieves' Tools) or Intelligence (Arcana) DC required to disable it. Describe what happens on a failure (does it trigger?).

## Output
Format the trap clearly in Markdown, using bolding for DCs, Saving Throws, and Damage so the DM can easily reference it during play.
",
        [".github/skills/travel-and-weather-generator/SKILL.md"] = @"---
name: travel-and-weather-generator
description: Generate travel times, random encounters, and extreme weather conditions for wilderness exploration.
---

# Travel and Weather Generator

Use this skill to flesh out a journey. Ask for the **Starting Location**, **Destination**, **Terrain Type**, and the party's **Pace** (Fast, Normal, Slow) if not provided.

## 1. Travel Time and Pace
- Calculate the rough travel time based on standard 5e travel paces (Fast = 30 miles/day, Normal = 24 miles/day, Slow = 18 miles/day).
- Apply terrain modifiers (e.g., difficult terrain halves speed).
- Note the mechanical effects of the pace (e.g., Fast pace imposes a -5 penalty to passive Perception).

## 2. Weather and Environment
- Generate prevailing weather for the journey.
- If extreme weather occurs (Extreme Cold, Extreme Heat, Heavy Rain, Strong Wind), clearly state the 5e mechanical penalties (e.g., Disadvantage on Wisdom (Perception) checks, Constitution saving throws to avoid Exhaustion).

## 3. Random Encounters
- Generate 1-3 thematic random encounters for the journey. Mix combat encounters with non-combat/flavor encounters (e.g., a ruined shrine, a traveling merchant, strange tracks).
- For combat encounters, provide a brief summary of the enemies and their motivation.

## Output
Format the travel details clearly. Use a Markdown table for the day-by-day journey or list the encounters logically so the DM can use them sequentially.
",
        [".github/skills/standee-sheet-creator/SKILL.md"] = @"---
name: standee-sheet-creator
description: Create consistent, print-ready D&D character standees (paper pawns/miniatures) for 3D-printed holders (16.6 mm slot width, 9 mm tab height) on a 2-page A4 landscape PDF with matching front and rear views.
---

# Skill: D&D Character Standee Sheet Creator

## Purpose

Create consistent, print-ready character standees for the D&D Campaign
project. The standees are printed on paper, optionally laminated, cut
out, and inserted into the user's 3D-printed holders.

The goal is to produce a clean, low-ink, double-sided A4 PDF with
matching front and back views, while avoiding unnecessary prompt
iterations.

## Design principles

1.  **The character is the product.** Do not add decorative page frames,
    ornate borders, background scenes, title banners, instructions,
    flourishes, or other artwork around the standees.
2.  **White background only.** Use a plain white page so the sheet is
    economical to print and easy to cut.
3.  **No wasted ink.** Keep colour and detail on the character itself.
    Avoid large filled areas, decorative panels, shadows on the page,
    and unnecessary gold ornamentation.
4.  **Preserve established character identity.** Use the
    supplied/reference portrait as the source of truth for face, age,
    hairstyle, build, clothing, accessories, colours, and overall
    personality. Do not redesign a character unless explicitly asked.
5.  **Full-body views.** Show the entire character, including shoes/feet
    and the complete outline of clothing, weapons, hair, hats, capes,
    and other accessories. Do not crop the head, hands, weapons, or hem.
6.  **Consistent collection style.** All characters on one sheet should
    share the same rendering style, lighting, detail level, scale
    convention, and clean cutout treatment.
7.  **Keep the character's natural silhouette.** A wide dress, cape, or
    weapon may extend beyond the width of the holder. Only the small
    insertion tab is constrained by the holder.

## Holder dimensions and insertion tab

The user's OpenSCAD holder has these relevant dimensions:

-   Base footprint: **25 mm × 10 mm**
-   Slot thickness: **0.5 mm**
-   Slot length: approximately **16.67 mm** (`stand_width * 2/3`)
-   Intended insertion depth: approximately **10 mm** (the holder's
    upright slot region)

Use these standee tab targets:

-   **Tab width:** 16.5--16.6 mm when laid out at final print size.
-   **Tab height:** approximately 9 mm.
-   **Tab thickness:** paper/lamination thickness has already been
    tested by the user and does not need to be modelled or questioned.
-   **Tab shape:** a simple, centered rectangular extension directly
    connected to the bottom of the character cutout.
-   **Tab label:** character name only, centered on the tab in small,
    legible type. A restrained dark serif font is suitable. A tiny
    divider may be used only if it does not make the tab crowded.
-   Do not add a separate pedestal, oversized blank base, decorative
    nameplate, or large white box beneath the character.

The tab must be part of the same continuous cutout silhouette as the
character. It is not a separate object.

## Sheet layout

Default output:

-   **Two-page PDF**
-   **A4 landscape**
-   Page 1: front views (recto)
-   Page 2: matching back views (verso)
-   Same character order on both pages
-   Same final physical scale and placement on both pages
-   Plain white page background
-   Comfortable spacing between cutouts so scissors can pass between
    them
-   Thin, unobtrusive cut contour around each standee; use a light
    neutral grey or dark grey hairline, not a decorative frame

For a small NPC collection, arrange characters in one horizontal row if
they fit legibly. If the characters become too small, use a balanced
grid while preserving identical positions between the front and back
pages.

Do not place page titles, collection labels, print instructions,
ornamental borders, or explanatory text on the print sheet. Put any
instructions in the chat response, not on the PDF.

## Front and back views

Each standee needs two corresponding views:

-   **Front:** the established character design, full body, facing
    mostly forward or in a slight three-quarter pose.
-   **Back:** a believable rear view of the same character, showing the
    back of the same outfit, hairstyle, cape, coat, dress, straps,
    belts, and accessories.

The back must not be a newly invented alternate costume. Maintain the
same clothing materials, colours, proportions, and accessories.

### Duplex alignment

The reverse page must be prepared for the intended printer duplex
setting. For A4 landscape, the expected setting in the current workflow
is **double-sided, flip on short edge**, with the reverse artwork
mirrored horizontally when required by the page assembly method.

Important: printer drivers vary. Do not claim alignment is guaranteed
solely because the page is mirrored. Keep identical scale, page
placement, and cutout centers on both sides. Recommend one plain-paper
test print before laminating the full sheet.

## Image-generation prompt requirements

When generating the artwork, explicitly state:

-   A clean, print-production character standee sheet on a pure white
    background.
-   The exact character count and names.
-   Full-body front views, with complete silhouettes and visible feet.
-   A matching back-view row showing the same characters in the same
    left-to-right order.
-   Each character isolated with generous white space between
    neighbouring cutouts.
-   A single continuous outer cut contour around each character and its
    integrated bottom tab.
-   A narrow bottom tab, approximately 16.6 mm wide by 9 mm high at
    final printed size.
-   The character name centered on the tab, and nowhere else.
-   No page decoration, no background environment, no decorative frame,
    no title, no extra labels, no instruction panel, no separate base,
    and no oversized white rectangles.
-   Consistent character identity and costume details based on the
    supplied references.

### Suggested reusable prompt

> Create a clean, print-production sheet of [NUMBER] tabletop RPG
> character standees on a pure white background. Use the supplied
> character reference images as strict identity and costume references.
> Each character must be shown full-body, with the complete head, hair,
> hands, weapons, clothing silhouette, and feet visible. Arrange the
> front views in one row, left to right in this exact order: [CHARACTER
> NAMES]. Below them, show matching full-body rear views in the exact
> same order, depicting the backs of the same outfits and hairstyles.
> Each figure must be an isolated cutout with generous white space
> around it and a thin, unobtrusive cut contour. At the bottom of each
> figure, continue the cutout directly into one small centered
> rectangular insertion tab, approximately 16.6 mm wide and 9 mm high at
> final A4 print size. Put only that character's name centered on the
> tab in small, legible dark serif lettering. The tab is part of the
> character cutout, not a separate pedestal. The character body may be
> any width; only the insertion tab must match the holder. Use a
> consistent cinematic Victorian-fantasy illustration style across the
> entire collection. Pure white page, minimal ink usage.
>
> **Strict exclusions:** no decorative page border, no ornate frame, no
> title banner, no headings, no instructions, no background scenery, no
> coloured page panels, no large gold decorations, no separate base, no
> oversized blank box under the feet, no extra text, and no cropped body
> parts.

### Important image-generation limitation

Image generators are useful for producing character artwork and visual
layouts, but they are not reliable tools for exact millimetre
dimensions, typographic accuracy, cut-line engineering, or duplex
registration. Do not rely on a generated image alone as the final
production PDF when precise physical sizing matters.

For best results:

1.  Generate or obtain each character's front and back artwork.
2.  Remove backgrounds and isolate the characters where needed.
3.  Assemble the sheet programmatically at the final physical
    dimensions.
4.  Add the tab geometry and name labels in the layout stage, not by
    asking the image generator to typeset them.
5.  Place the reverse images in matching positions and apply the
    required duplex mirroring.
6.  Export a two-page A4 landscape PDF.
7.  Verify the PDF page count, page dimensions, and tab width before
    delivery.

## Quality-control checklist

Before delivering the PDF, verify:

-   [ ] Exactly two pages: front and back.
-   [ ] A4 landscape page size.
-   [ ] Plain white background with no decorative page furniture.
-   [ ] Every character is full-body and not cropped.
-   [ ] Correct character identity and costume details.
-   [ ] Same character order on both pages.
-   [ ] Back views are genuine rear views of the same designs.
-   [ ] Each character has one integrated tab.
-   [ ] Tab width measures approximately 16.5--16.6 mm in the final PDF.
-   [ ] Tab height is approximately 9 mm.
-   [ ] Character name appears only on its tab and is readable.
-   [ ] No oversized white base rectangles or separate pedestals.
-   [ ] Cut contours are simple, visible, and easy to follow.
-   [ ] Front and back use the same scale and page placement.
-   [ ] Reverse page is prepared for landscape duplex printing, with
    mirroring checked.
-   [ ] A test print is recommended before laminating.

## Delivery notes

When presenting the result, include:

-   A direct PDF link.
-   A short note that the PDF has two pages (front and back).
-   Print guidance: A4 landscape, double-sided, flip on short edge
    (subject to printer test).
-   A reminder to test one sheet before laminating.

Do not claim exact duplex alignment is guaranteed without a physical
printer test.
",
        ["AGENTS.md"] = @"**CRITICAL RULE:**
At the start of any task, you MUST always ask the user if he want to use the campaign master persona. If yes, you MUST use your file tools to read `.github/agents/campaign-master.agent.md` and adopt that persona before proceeding. If no, you MUST use your default persona.
",
    };

    /// <summary>
    /// Ensures all AI templates exist in the target campaign content store. Missing or modified templates are written.
    /// </summary>
    public static async Task<int> SyncAsync(ICampaignContentStore contentStore, CancellationToken cancellationToken = default)
    {
        var syncedCount = 0;
        foreach (var (relativePath, content) in Files)
        {
            var exists = await contentStore.ExistsAsync(relativePath, cancellationToken);
            if (!exists)
            {
                await contentStore.WriteTextAsync(relativePath, content, cancellationToken);
                syncedCount++;
            }
            else
            {
                var existing = await contentStore.ReadTextAsync(relativePath, cancellationToken);
                if (!string.Equals(existing, content, StringComparison.Ordinal))
                {
                    await contentStore.WriteTextAsync(relativePath, content, cancellationToken);
                    syncedCount++;
                }
            }
        }
        return syncedCount;
    }
}
