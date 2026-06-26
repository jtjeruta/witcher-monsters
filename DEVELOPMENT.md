# Witcher Monster Quests — Developer Notes

Internal documentation for contributors. **Spoils gameplay mechanics** — keep this out of the player-facing README.

## Scope (v1)

- **Leshen** contract beast (`WitcherMonsters_Leshen`) with vine spine launch, wolf summons, and forest skip retreat
- **Contract beast hediff** — dormant → activated manhunter + pacification blocks (Harmony)
- **Gorehulk-style AI** via custom `Leshen` think tree (Anomaly job givers)
- **Dev spawn** via debug action or dev spawn-pawn menu; contract hediff auto-applied on leshen spawn

Deferred: quest script integration, Mountains biome pool swap, additional Anomaly entity pool entries.

## Dependencies

Requires **Biotech**, **Anomaly**, and **Witcher Base**. Leshen AI uses `JobGiver_AIAbilityFight`, `Ability.GetJob` for forest skip retreat, and `JobGiver_ShamblerWander`. Threat detection via `LeshenThreatUtility` (player colonists, humanlikes, hostiles).

## Contract beast

`WitcherMonsters_ContractBeast` hediff (hidden label) is **auto-applied** to every leshen on map spawn (dev spawn-pawn, debug action, quests).

- **`HediffComp_LeshenDormant`** — scans every 30 ticks for player/hostile pawns within **12** cells; on first detection sets `activated`, applies `ManhunterPermanent`, shows “The leshen stirs.” Also syncs `activated` if vanilla manhunter triggers from damage.
- **`HediffComp_ForceManhunter`** — once activated, re-applies `ManhunterPermanent` every **120 ticks** (covers Axii/calm edge cases)
- `ContractBeastUtility.IsContractBeast(Pawn)` / `LeshenUtility.IsActivated(Pawn)` for future quest code

Harmony patches (`Patch_BlockPacification.cs`):

- `WitcherBase.CompAbilityEffect_TameAnimal.CanApplyOn` — blocks Axii on contract beasts
- `CompAbilityEffect_StopManhunter.CanApplyOn` / `.Apply` — blocks ideology Animal Calm

Ships **0Harmony.dll** in `Assemblies/` (Lib.Harmony 2.3.3).

## Leshen AI

Custom think trees in `Defs/ThinkTreeDefs/WitcherMonsters_Leshen.xml`:

| State | Behavior |
|-------|----------|
| **Dormant** | `JobGiver_ShamblerWander` (radius 3) + `JobGiver_Idle` — tree-like, non-aggressive |
| **Activated** | `ThinkNode_ConditionalLeshenRetreat` (foe within 8 → `WitcherMonsters_LeshenSkip` away only), else `JobGiver_AIAbilityFight` for wolves + `WitcherMonsters_LeshenSpineLaunch` |

No `JobGiver_Manhunter` or `JobGiver_AIFightEnemies` — leshen never melee-charges.

Spawn via `MonsterSpawnUtility` adds contract hediff only (no immediate manhunter).

## Leshen tuning

| Knob | Location | Default |
|------|----------|---------|
| Activation radius | `HediffCompProperties_LeshenDormant` | 12 cells |
| Combat power | `PawnKindDef` | 350 |
| Move speed | `ThingDef` | 1.0 |
| Body size / health | `ThingDef.race` | 1.6 / 3.0 scale |
| Spine launch | `WitcherMonsters_LeshenSpineLaunch` (Gorehulk projectile) | range 17.9, cooldown 130 |
| Forest skip | `WitcherMonsters_LeshenSkip` (EntitySkip child, aiCanUse false) | cooldown 1200–2400 ticks (20–40 s), max hop 19.9 |
| Skip retreat trigger | `JobGiver_LeshenSkipRetreat` | 4 cells |
| Retreat branch radius | `ThinkNode_ConditionalLeshenRetreat` | 8 cells |
| Skip min/max hop | `LeshenSkipRetreatUtility` | 4–19.9 cells, +3 min distance from threat |
| Wolf count | `CompProperties_AbilitySummonWolves` | 2–3 |
| Wolf summon acquire radius | `JobGiver_AIAbilityFight` | 12 |
| Spine acquire radius | `JobGiver_AIAbilityFight` | 17 |
| Dormant wander radius | `JobGiver_ShamblerWander` | 3 |

Summoned wolves get `ManhunterPermanent` only (not the contract hediff). AI skips summon when **≥2** manhunting timber wolves are already within 12 cells and no hostiles are nearby.

Leshen drops **40 wood** and **30 twisted meat** on butcher (`MeatAmount` + `specificMeatDef`, plus `butcherProducts` wood). Corpses use `EntityFlesh` / `CorpsesEntity` (entity butcher tab, not animal meat). No leather (`LeatherAmount` 0). `foodType` is `None` and `baseHungerRate` is 0. Melee tools have `chanceFactor` 0.

## Art (placeholder → custom)

Until PNGs exist, the leshen uses **vanilla willow tree** textures (`Things/Plant/TreeWillow`, leafless for dessicated) with `drawSize` **2.8**. Trees do not rotate with facing — acceptable until custom art lands.

**Folder:** `Textures/Things/Pawn/Animal/Leshen/`

| File | Size |
|------|------|
| `Leshen_east.png` | 512×512 RGBA |
| `Leshen_north.png` | 512×512 |
| `Leshen_south.png` | 512×512 |
| `Leshen_west.png` | 512×512 |
| `Dessicated_Leshen_*.png` | 512×512 ×4 directions |

Optional ability icons (256×256): custom leshen art when ready.

Update `texPath` in `Defs/ThingDefs_Races/WitcherMonsters_Leshen.xml` when art lands.

## Anomaly extension

`Optional/Anomaly/README.md` — reserved for future additional Anomaly pool defs. Core leshen now requires Anomaly for AI job givers.

## Build

```bash
cd Source && ./build.sh
```

Output: `Assemblies/WitcherMonsters.dll` + `Assemblies/0Harmony.dll`.

Symlink to RimWorld `Mods/WitcherMonsters`. Enable Biotech, Anomaly, Witcher Base, Witcher Monster Quests.

## Test plan

1. Dev spawn leshen → dormant willow tree; colonists safe at long range
2. Walk within ~12 cells → “The leshen stirs.”, combat begins
3. At range → Gorehulk spine launch + wolf summon; no charge into melee
4. Rush within ~4 cells → forest skip teleports away; no spine/wolves until foe is >8 cells
5. After activation → stays hostile; Axii and Animal Calm fail
6. Butcher yields 40 wood + 30 twisted meat; leshen does not eat
7. Mod loads with Biotech + Anomaly + Witcher Base
