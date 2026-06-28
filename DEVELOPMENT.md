# Witcher Monster Quests — Developer Notes

Internal documentation for contributors. **Spoils gameplay mechanics** — keep this out of the player-facing README.

## Scope (v1)

- **Black blood / revenant counter** — Harmony patches (requires Witcher Potions + Anomaly): block `CompRevenant.Hypnotize` when victim has `WitcherPotion_BlackBloodHediff`; revenant backlash applies vanilla `StunHandler.StunFor` (900 ticks / ~15s, same mechanism as Aard) plus forced reveal; light melee contact `ToxicBuildup` via `Pawn.PostApplyDamage` postfix (`totalDamageDealt` param)
- **Leshen** contract beast (`WitcherMonsters_Leshen`) with vine spine launch, wolf summons, and forest skip retreat
- **Contract beast hediff** — dormant → activated manhunter + pacification blocks (Harmony)
- **Monster contracts** — random world quests + settlement caravan gizmo; villager-flavor text hides the real beast
- **Contract sites** — massacre scenes (cart / camp / corpse field) + encounter-specific hostiles
- **Rewards** — silver drop pods, faction goodwill, or Law of Surprise (7-day delayed gift)
- **11 core encounter pools** + optional **7 Dark Ages** pools when `Van.Beasts` is active

## Dependencies

Requires **Biotech**, **Anomaly**, and **Witcher Base**. Leshen AI uses `JobGiver_AIAbilityFight`, `Ability.GetJob` for forest skip retreat, and `JobGiver_ShamblerWander`. Threat detection via `LeshenThreatUtility` (player colonists, humanlikes, hostiles).

Optional Dark Ages encounter defs live in `ContractData/WitcherMonsters_ContractEncounters_DarkAges.xml` with `MayRequire="Van.Beasts"`.

## Monster contracts

### Sources

| Source | How |
|--------|-----|
| Random quest | Root quest `WitcherMonsters_ContractHunt` (`rootSelectionWeight` 0.85, `rootMinWealth` 3000) |
| Settlement board | Caravan gizmo **Hear monster contracts** on friendly non-player settlements |

Any colonist caravan can accept; no witcher pawn required. Requires **`Witcher_Traditions`** research (Witcher Base) — random world offers and settlement gizmos stay hidden until it is finished.

### Data defs

Contract encounter and flavor data live in **`ContractData/`** (not `Defs/`) and load via `ContractDataLoader` at startup. They are plain data classes, not RimWorld `Def` subclasses — this avoids Def-parser registration issues.

- **`ContractEncounterDef`** — spawn pool (`spawnEntries`, `selectionWeight`, `requiredModPackageId`, rewards)
- **`ContractFlavorDef`** — villager voice (`questTitle`, `questDescription`, optional `preferredScene`); never names the real monster

Core pools: wolves, leshen, bear, sightstealers, shamblers, ghouls, gorehulk+shamblers, chimeras, devourers, revenant, metalhorror.

### Site generation

`SitePartDef` `WitcherMonsters_ContractSite` tag `WitcherMonsters_Contract`:

- `GenStep_ContractScene` — abandoned cart / camp / corpse field + human corpses
- `GenStep_ContractThreats` — `ContractThreatSpawner` (manhunter, entity faction, leshen dormant via contract hediff)

`WorldObjectComp_ContractSiteData` on `Site` stores encounter/flavor/reward from quest slate.

### Quest flow

1. `QuestNode_EnsureContractOffer` rolls encounter + flavor + reward (or uses pre-filled slate from settlement board)
2. `Util_GenerateSite` → contract site tile
3. `QuestNode_SetContractSiteData` copies offer onto site comp
4. `QuestPart_ContractKillWatcher` registers with `MapComponent_ContractSite` on `site.MapGenerated`
5. Map tick watches registered contract threats; on all dead → reward + `WitcherMonsters_ContractComplete` signal
6. Timeout 12–28 days; fail on site destroyed

### Rewards

| Type | Delivery |
|------|----------|
| Silver | Drop pods to player home map |
| Goodwill | +goodwill with origin settlement faction (or nearest friendly) |
| Law of Surprise | `GameComponent_ContractSurprise` — 7 days later: silver/gold, farm animal, or child pawn (8–12) |

Settlement board shows reward type before accept (Law of Surprise labeled mysteriously).

### Settlement board

Mirrors Witcher Base orphan-stock pattern: `WorldObjectComp_ContractBoard` on `Settlement`, 2–4 offers, restock every ~12 days.

## Contract beast (leshen)

`WitcherMonsters_ContractBeast` hediff (hidden label) is **auto-applied** to every leshen on map spawn (dev spawn-pawn, debug action, quests).

- **`HediffComp_LeshenDormant`** — scans every 30 ticks for player/hostile pawns within **12** cells; on first detection sets `activated`, applies `ManhunterPermanent`, shows “The leshen stirs.” Also syncs `activated` if vanilla manhunter triggers from damage.
- **`HediffComp_ForceManhunter`** — once activated, re-applies `ManhunterPermanent` every **120 ticks** (covers Axii/calm edge cases)
- `ContractBeastUtility.IsContractBeast(Pawn)` / `LeshenUtility.IsActivated(Pawn)` for contract code

Contract beast pacification blocks (Axii tame/calm) live in **Witcher Base** via `WitcherMonstersInterop` + `CompAbilityEffect_StopManhunterContractAware` — no Harmony in this mod.

Leshen contract hediff auto-apply uses `GameComponent_ContractBeastSweep` (periodic scan) plus spawn paths in `MonsterSpawnUtility` / `ContractThreatSpawner`.

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

Output: `Assemblies/WitcherMonsters.dll` only (no bundled Harmony). Black blood patches compile against `Lib.Harmony` NuGet; runtime Harmony comes from the mod manager / HugsLib like other RimWorld mods.

Symlink to RimWorld `Mods/WitcherMonsters`. Enable Biotech, Anomaly, Witcher Base, Witcher Monster Quests.

## Test plan

### Leshen

1. Dev spawn leshen → dormant willow tree; colonists safe at long range
2. Walk within ~12 cells → “The leshen stirs.”, combat begins
3. At range → Gorehulk spine launch + wolf summon; no charge into melee
4. Rush within ~4 cells → forest skip teleports away; no spine/wolves until foe is >8 cells
5. After activation → stays hostile; Axii and Animal Calm fail
6. Butcher yields 40 wood + 30 twisted meat; leshen does not eat

### Monster contracts

1. Random contract quest appears; flavor text does not name the beast
2. Settlement gizmo shows board, restocks, accepts without witcher pawn
3. Contract site has massacre scene + correct hostiles per pool
4. Leshen contract: dormant until approach, then kill completes quest
5. Silver / goodwill / Law of Surprise each fire once on completion
6. With Van.Beasts: Dark Ages pools appear; without, skipped
7. Mod loads with Biotech + Anomaly + Witcher Base
