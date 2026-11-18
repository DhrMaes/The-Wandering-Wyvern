---
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
