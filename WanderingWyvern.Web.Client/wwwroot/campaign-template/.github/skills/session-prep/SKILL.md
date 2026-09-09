---
name: session-prep
description: Prep a new D&D campaign session (outline.md + script.md) for this repo. Use when asked to prep, plan, or start prepping a new session, or to add scenes/content to an existing session's outline or script.
---

# Session prep

Prep a session under `Sessions/session-##-title/`, following the lessons below (learned from actually running sessions at the table).

## What goes where

- **`outline.md`** — DM-facing regie notes: scene structure, decision points, mechanical prep (encounters, DCs, loot), pacing, and a "Benodigde NPCs/Locaties/Items" links section. Start with a `**Datum:** YYYY-MM-DD` metadata line.
- **`script.md`** — everything the DM actually needs to *run* the session out loud. Build it side by side with `outline.md` from the start — never bolt it on later.

## `outline.md` structure — keep it short and to the point

`outline.md` is a quick-glance regie reference, **not** a second script. Follow this shape:

1. `# Sessie <nummer> — <Titel>` (Example: `# Sessie 4 — Echolocation`)
2. `**Sessie:** <nummer>`
3. `**Datum:** YYYY-MM-DD` — or `**Datum:** TBD` if not scheduled yet.
4. `**In-Game Datum:** [in-game datum]`
3. `## Doel van de sessie` — one or two sentences on what this session needs to accomplish (the emotional/plot beat, not a scene-by-scene recap).
4. One short section per scene, each just a **quick, decisive glance**: what happens, key checks/DCs, and links to the NPCs/locations/items involved. A sentence or two per scene is usually enough — the *how to say it* content belongs in `script.md`, not here. Don't restate read-aloud text or write out full dialogue in the outline.
5. A **"Benodigde NPCs/Locaties/Items"** section linking everything needed for the session.

Avoid the anti-pattern seen in `Sessions/session-02-the-caged-dragon/outline.md`, which grew into a mix of outline *and* script (full DC tables, extended prose, near-duplicate scene descriptions) — that made it a second script.md instead of a quick reference. Keep new outlines lean; put all the atmosphere, exact wording, and branching detail in `script.md` instead.

## `script.md` structure — the DM's actual table tool

1. `# Sessie ## — Voorleesscript voor de DM`
2. `**Datum:** YYYY-MM-DD` (or `TBD`) plus a short italic companion note pointing back to `outline.md`.
3. One `## Scène N — Titel` section per scene, in play order, each containing:
   - The **literal read-aloud text**, wrapped in `> blockquote`, ready to say almost verbatim.
   - Regie notes around it: what to do, what checks to call for, and how to react to results.
   - **Branch on roll results explicitly, don't just gate pass/fail.** For any check where the outcome should scale (e.g. searching a room, gathering information, persuading someone), write out what happens at meaningful DC tiers, not just success/failure — for example:
     > *Investigation-check om de kamer te doorzoeken:*
     > - **DC 10 (bv. een 7 totaal):** ze merken de voor de hand liggende dingen op — een omgekeerde stoel, een opengebroken kast.
     > - **DC 14:** ze vinden bovendien een verborgen laatje met een half verbrande brief.
     > - **DC 18+:** ze zien ook het kleine detail dat alles verraadt — het slot is van binnenuit geforceerd, niet van buiten.

     This is the place to think ahead and be creative: sketch out a few tiers of "what they notice/learn" ahead of time so you're not improvising the gradient live at the table.
   - Optional side-checks/side-content clearly marked **"Optioneel:"**, wrapped in a collapsible `<details><summary>Optioneel: ...</summary> ... </details>` block (rendered natively by the Campaign Viewer) so the script stays uncluttered at the table and can be expanded only if that thread comes up, e.g.:
     > ```html
     > <details>
     > <summary>Optioneel: Oswalt vraagt om verontschuldiging</summary>
     >
     > *(volledige uitwerking van dit spoor)*
     > </details>
     > ```

## Hard rules learned from actually running sessions

1. **`script.md` needs the exact wording, not a summary.** When a scene, room, or item needs to be described to the players, write the literal text the DM should read/paraphrase aloud — full sentences, in the campaign's content language, ready to use at the table. Don't just note "describe the room" — write the description itself. Wrap read-aloud text in `> blockquote` (the Campaign Viewer renders these specially, so read-aloud text is visually obvious mid-session).
2. **No clutter, no dead information.** Only include what's actually needed to run *this* session. Do **not** add notes like "Rita is not present this session" or other negative/non-actionable trivia — if an NPC/location/item isn't part of the session, simply don't mention it. Every line in `script.md` should be something the DM will actually say or use.
3. **Zero session-specific content lives in the entity files.** NPCs/Locations/Items/Lore/PCs must stay strictly evergreen. If you need session-specific behavior for an existing NPC/Location, put it directly in `outline.md`/`script.md` — never a separate file in the session folder (see below), and never in the NPC's/Location's own file. This was a real pain point: flipping through character files mid-session for stuff that should've been in the session prep.
4. **Reference entities by link, don't duplicate them.** Now that Markdown links and inline `` `Path/file.md` `` code-spans both render as clickable modal popups in the Campaign Viewer, use those instead of copy-pasting an NPC's/location's/item's details into the session file. Link out; only inline the parts that are genuinely session-specific (like "today, Robert also mentions...").
5. Keep expanding the "Benodigde NPCs/Locaties/Items" list in `outline.md` as prep continues — don't leave it to patch in later, and don't leave it to the session itself.
6. For every NPC/Location/Item referenced that doesn't exist yet, create a stub now (matching the entity's own folder/file conventions) rather than leaving a dangling reference.
7. Write all actual campaign content in whichever language this campaign's `copilot-instructions.md` specifies.

Do not create `recap.md` yet — that's written after the session.

## Folder/file naming

`Sessions/session-##-title/` — kebab-case, zero-padded two-digit session number (e.g. `session-03-the-broken-seal`).

## A session folder always has exactly these 4 files

`Sessions/session-##-title/` contains **only**: `outline.md`, `script.md`, `live-notes.md` (created empty/ready before the session, filled live during play via the Campaign Viewer), and `recap.md` (written after the session). Do not add extra session-scoped files (e.g. a separate `<npc-slug>.md` for one NPC's session-specific behavior) — that content belongs directly inside `outline.md`/`script.md` instead.
