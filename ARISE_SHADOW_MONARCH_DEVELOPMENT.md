# Shadow Monarch — RimWorld Mod Development Specification

> Implementation brief for AI agent. Target RimWorld 1.6. Implement a faithful, persistent shadow army inspired by Solo Leveling. This is a specification, not a claim that the described APIs have already been verified.
Name of the mod: ARISE: Shadow Monarch
Author: TOLGRIM

## 1. Mission and non-negotiable scope

Build a RimWorld mod in which a designated pawn (the **Monarch**) can extract a defeated pawn's shadow from its corpse, store that shadow outside the active map, summon it as a loyal combatant, recall it, and recover it after combat defeat. Shadows are permanently bound to their Monarch while the bond exists. They do not eat, sleep, age, perform colony labor, or suffer mental breaks. They should preserve the source pawn's identity, race, appearance silhouette, and combat-relevant capabilities where safely possible.

**Do not implement ranks, rank assignment, rank progression, experience, levels, promotion, creature power scoring, or automated classification of creature strength.** A shadow's strength comes from its original pawn and existing game stats. Do not add an artificial power-tier system. No new buildings. No dependency on Biotech or mechanitor bandwidth. Avoid adding mana, cooldowns, army size limits, teleportation, domains, or other powers until the core lifecycle is stable; these are explicitly out of MVP scope.

## 2. Working rules for Codex

1. Inspect the repository first: current mod structure, supported RimWorld version, dependencies, existing patches, build tooling, and conventions. Preserve working code and avoid unrelated refactors.
2. Verify actual RimWorld 1.6 method signatures and save/load behavior from the user's legally installed assemblies or established local references. **Do not invent APIs.** If assemblies are unavailable, create a documented integration blocker instead of guessing.
3. Implement incrementally, with one small vertical slice at a time; compile and test each slice before proceeding.
4. Keep Harmony patches narrow and justified. Prefer supported Defs, comps, abilities, jobs, and existing engine behavior before patching core methods.
5. No destructive changes to existing saves; provide migration/versioning for persisted records. Never silently delete a shadow when a record is invalid.
6. Use English identifiers, code comments, XML labels, and documentation. Prepare localization keys for English and Spanish, without hard-coding player-facing text.
7. Keep an updated `CHANGELOG.md` and a short `docs/IMPLEMENTATION_NOTES.md` with verified API assumptions, test results, known limitations, and decisions. Do not claim tests passed unless actually executed.
8. Avoid broad speculative frameworks. Introduce abstractions only where they protect persistence, ownership, lifecycle transitions, or compatibility.

## 3. Player-facing gameplay

### 3.1 Monarch designation

- Provide a simple, explicit way to designate a living player-controlled pawn as the Monarch (initially a dev-mode action or a simple ability/hediff is acceptable; choose the least invasive verified implementation).
- The designation persists across saves and maps.
- Support exactly one Monarch in the MVP, but use a stable pawn identity in the data model so multi-Monarch support is not structurally impossible.
- If the Monarch becomes unavailable or dies permanently, preserve shadow records, mark the army unavailable, and prevent summoning until a documented future restoration mechanic exists. Do not destroy records.

### 3.2 Shadow extraction — Arise

- A player-controlled Monarch can target a valid corpse using an ability or equivalent vanilla-compatible command.
- The corpse must reference a dead pawn, must not already have yielded a shadow, and must not have been irreversibly destroyed.
- Each corpse has **three extraction attempts maximum**, persisted across save/load. Failed attempts consume one attempt. Success consumes the opportunity and produces exactly one shadow record.
- Extraction chance should use a **simple configurable constant** for the MVP (e.g., 70% per attempt), not creature stats, ranks, race scoring, age, combat power, or elaborate formulas. Allow changing this value in settings or XML. Display the chance and attempts remaining.
- On success, the original corpse must no longer be available as a source for additional extraction. Handle disposal or marking without duplicating loot or resurrecting the physical corpse unexpectedly.
- If a corpse or pawn cannot be safely converted, reject extraction with a clear reason, without consuming an attempt. Never corrupt the source pawn.
- Do not assume that every modded pawn or mechanoid can be supported; fail safely and document exceptions.

### 3.3 Shadow identity and equipment

- Prefer preserving the original pawn instance when safe, rather than regenerating a generic pawn from its PawnKindDef.
- Preserve name, race, body configuration, relevant skills, attacks, and supported combat abilities.
- Treat incompatible needs, hediffs, genes, relations, faction membership, and special mod components explicitly. Do not indiscriminately clear all health conditions or delete mod-owned data.
- **Never duplicate equipment or inventory.** Physical loot remains subject to normal game rules. For MVP, prioritize unarmed/natural-weapon combat or a safe non-lootable shadow weapon representation. If a robust equipment solution is unavailable, document the limitation and do not clone items.
- A shadow must not become an ordinary colonist, count as a player population pawn for unrelated events, or participate in normal social/work systems.

### 3.4 Storage, summon, recall

- Stored shadows do not occupy map cells and do not execute regular pawn ticks, jobs, needs, or AI.
- The Monarch can open a minimal army UI showing each shadow's name, original race, state, and summon/recall action.
- Summon only on a valid reachable cell in a loaded map with explicit validation. A summoned shadow appears once, under the Monarch's command, without duplicate references.
- Recall removes the active shadow safely from the map and returns it to storage, preserving its identity and relevant state.
- The army remains associated with the Monarch during caravan travel and map transitions. If summoning into caravans is not safely supported, disable that action and keep storage available.

### 3.5 Combat, defeat, recovery

- Shadows are absolutely loyal, hostile to the Monarch's enemies, and incapable of rebellion, mental breaks, social interactions, or colony work.
- MVP orders: follow/protect Monarch, attack hostile targets, hold position, recall. Reuse verified vanilla jobs, drafting, and targeting where possible.
- A shadow defeated in combat must not be permanently lost. Its lifecycle transitions to `Recovering`, then back to `Stored` when safe. For MVP, use a simple configurable recovery duration; do not introduce a mana system.
- Death interception must handle relevant damage and destruction paths without leaving ghost pawns, corpses, broken trackers, or duplicate records. Avoid blanket `Pawn.Kill` prefixes that break other mods. If safe interception is not yet verified, ship a smaller tested subset and document the blocker.
- Restoration should remove battle injuries as appropriate but preserve original identity and stable combat attributes. Ensure the recovery operation is idempotent.

### 3.6 Visual presentation

- Apply a recognizable dark/purple shadow appearance to summoned pawns while preserving silhouettes. Start with low-cost tint/effects and verify rendering hooks before complex shader work.
- Extraction and summon effects should be short-lived; do not continuously spawn costly particles.
- Keep visual work separate from core lifecycle and serialization. The mod must function if effects are disabled.

## 4. Technical architecture

Use C# + XML Defs + Harmony where required. Suggested modules (names are illustrative, not verified vanilla types):

```text
Source/ShadowMonarch/
  Core/                  # Mod bootstrap, settings, DefOf, versioning
  Monarch/               # Monarch identity and eligibility
  Extraction/            # Validation, attempt tracking, conversion
  Shadows/               # ShadowRecord, registry, lifecycle, storage
  Abilities/             # Arise, summon, recall
  Combat/                # Orders, protection, defeat/recovery
  Rendering/             # Shadow tint and transient VFX
  UI/                    # Minimal shadow army management
  Compatibility/         # Safe feature detection and exceptions
  Patches/               # Small, targeted Harmony patches
Defs/
  AbilityDefs/
  HediffDefs/
  EffecterDefs/
Languages/
  English/
  Spanish/
Tests/                    # Test plans and any runnable harnesses
```

### 4.1 Persistent model

Define a stable `ShadowId`, an owner/Monarch identity, a reference to the source pawn, lifecycle state, recovery deadline/ticks, and a schema version. Define extraction attempt tracking separately and bind it to the actual corpse or a stable persisted source identity.

States:

- `Stored`: held in the shadow registry, not spawned.
- `Summoned`: spawned on exactly one map.
- `Recovering`: temporarily unavailable after defeat.
- `Unavailable`: Monarch/bond unavailable; record retained.

Transitions:

```text
Corpse --successful Arise--> Stored
Stored --Summon--> Summoned
Summoned --Recall--> Stored
Summoned --Defeat--> Recovering
Recovering --Recovery complete--> Stored
Stored/Summoned/Recovering --Monarch unavailable--> Unavailable
Unavailable --future verified restoration--> Stored
```

Every transition goes through a single lifecycle service. Make transitions atomic where possible, validate preconditions, and reject duplicate requests. Do not allow UI, ability effects, and Harmony patches to write state independently.

### 4.2 Pawn ownership and serialization — critical

- Research and document RimWorld's actual `Scribe` semantics for spawned pawns, despawned/world pawns, caravans, and `GameComponent` data.
- **Never deep-save the same Pawn twice** via both map/world and registry. The record's serialization strategy must depend on whether the pawn is owned by the map/world or the shadow storage system.
- Do not assume `Scribe_Deep.Look(ref pawn, ...)` is universally correct. Use a verified owner/reference scheme with post-load resolution and invariant checks.
- After loading, reconcile records and spawned pawns by stable IDs. Detect duplicates, missing references, invalid states, and partial transitions; log actionable diagnostics and preserve recoverable data.
- Verify that stored pawns are truly removed from active simulation without being garbage-collected, destroyed, or reintroduced by vanilla systems.

### 4.3 Faction, needs, health, and AI

- Determine a reliable method to represent absolute loyalty without accidentally treating shadows as normal colonists. Avoid forcing every shadow into the player faction unless tests show this does not break colony population, alerts, caravans, work tabs, or incidents.
- Disable/override biological needs, age, mood, and mental states using verified hooks or selective comps, not unconditional per-tick scans.
- Preserve modded creature combat where possible. Use capability checks rather than hard-coded race lists or numeric power ratings.
- Prefer event-driven transitions and cached lookups; avoid iterating all shadows every tick. Recovery can be processed at coarse intervals or via recorded deadlines.

## 5. Implementation phases and acceptance gates

### Phase 0 — Repository and API audit

**Tasks:** inspect repository; determine mod package ID, RimWorld 1.6 dependencies, build commands, assembly references, and existing patch interactions; create a minimal technical decision log.

**Gate:** mod compiles in the existing environment; actual game API assumptions are recorded. Do not proceed with guessed APIs.

### Phase 1 — Monarch + registry

**Tasks:** persistent Monarch designation; stable shadow IDs; `GameComponent` registry; states; versioned save/load; diagnostics.

**Gate:** save/reload retains Monarch identity and empty registry; no errors in game log.

### Phase 2 — Extraction vertical slice

**Tasks:** Arise command, corpse validation, three persistent attempts, fixed configurable success chance, one shadow record per source, safe corpse handling. Start with a vanilla human pawn.

**Gate:** repeated attempts, success, failure, save/reload, invalid targets, and duplicate extraction behave correctly.

### Phase 3 — Storage, summon, recall

**Tasks:** storage ownership; safe spawn cell selection; map transitions; basic management UI; idempotent summon/recall.

**Gate:** one human shadow survives 20 summon/recall cycles and multiple save/reload cycles without duplicates, loss, or errors.

### Phase 4 — Combat and immortality

**Tasks:** combat loyalty, basic orders, defeat handling, recovery state, health restoration, no ordinary work/social/needs.

**Gate:** repeated lethal damage and recovery never permanently delete or duplicate the shadow; no broken corpse or equipment duplication.

### Phase 5 — Broader vanilla pawn support

**Tasks:** test animals, melee/ranged humans, different body types, wounds, gear, caravan travel, map removal, Monarch death. Add safe fallbacks.

**Gate:** each supported category passes extraction-to-recovery lifecycle; unsupported categories are rejected without save corruption.

### Phase 6 — Visuals and compatibility

**Tasks:** tint and brief effects; compatibility checks for selected modded races/creatures; profiling; localized text; user-facing configuration.

**Gate:** graphics are optional; no significant idle cost from stored shadows; no unexplained exceptions or duplicate pawns.

## 6. Mandatory test matrix

| Scenario | Expected result |
|---|---|
| Extraction failure | Attempts decrease by one; corpse remains valid until exhausted |
| Extraction success | Exactly one shadow registered; source cannot be extracted again |
| Save after failure | Remaining attempts preserved |
| Save while stored | Shadow preserved without active simulation |
| Save while summoned | Exactly one pawn on load; registry references it |
| Save while recovering | Recovery state and timing preserved |
| Repeated summon click | No duplicate spawns |
| Repeated recall click | No double despawn or record loss |
| Lethal damage | Shadow recovers, never permanently disappears |
| Physical gear on source | No duplicated loot or tradable shadow equipment |
| Monarch death | Records retained; summon disabled |
| Caravan/map change | Registry survives; unsafe summon disabled gracefully |
| Unsupported modded creature | Clear rejection; no crash or data corruption |
| Large stored army | No per-tick AI or needs processing for stored shadows |

For each phase, test both a fresh game and save/load where applicable. Record the actual game log, build status, and manual steps. Do not mark tests as passed without running them.

## 7. Performance and compatibility constraints

- **No ranks, levels, XP, creature scoring, or strength calculations.** This is a deliberate design choice, not a postponed feature.
- No scanning every corpse on every tick. Validate only when an ability targets a corpse or when the UI requests information.
- No periodic processing of stored pawns. Use sparse recovery checks or deadline-based bookkeeping.
- No always-on particle systems for stored shadows.
- No new buildings, workbenches, research trees, or mechanitor bandwidth integration.
- Use feature detection for external mods; never assume arbitrary modded races support the same health/death/rendering semantics.
- Harmony patches must be scoped, documented, and tested against vanilla first.

## 8. Explicitly out of scope

Do **not** implement any of the following without a later request:

- Shadow ranks, tiers, promotions, levels, XP, or automatic creature classification.
- Mana or elaborate combat-power formulas.
- Shadow Exchange, Monarch's Domain, mass extraction, bosses, story quests, or progression trees.
- New buildings, resources, or equipment crafting.
- Guaranteed support for every modded creature, mechanoid, or special entity.
- Multiplayer synchronization.

## 9. Definition of done

The MVP is complete when a designated Monarch can successfully extract a vanilla human enemy from a corpse (with three attempts), store the original identity safely, summon the shadow, command it in combat, recall it, recover it after defeat, and preserve the entire lifecycle across save/load and map transitions, **without duplicate pawns or equipment, corrupted saves, colony-work behavior, or permanent shadow death**.

## 10. Final deliverables from Codex

1. Compilable RimWorld 1.6 mod source and required XML Defs.
2. Working mod package layout and build instructions appropriate to the existing repository.
3. English and Spanish localization keys for all implemented UI strings.
4. Concise `CHANGELOG.md` updates and `docs/IMPLEMENTATION_NOTES.md`.
5. Phase-by-phase test evidence, known limitations, and unresolved compatibility issues.
6. No unrelated changes, speculative systems, or unsupported claims of successful tests.

**Start with Phase 0. After inspecting the project, implement the smallest verified vertical slice. Prioritize lifecycle correctness and save safety over visual polish.**
