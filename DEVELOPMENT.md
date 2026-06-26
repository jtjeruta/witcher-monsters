# Witcher Monster Quests — Developer Notes

Internal documentation for contributors. **Spoils gameplay mechanics** — keep this out of the player-facing README.

## Scope (v1)

- **Leshen** contract beast (`WitcherMonsters_Leshen`) with three Biotech-style abilities
- **Contract beast hediff** — permanent manhunter + pacification blocks (Harmony)
- **Dev spawn** via debug action; no quest generator yet

Deferred: quest script integration, Mountains biome pool swap, Anomaly pawn pool entries.

## Contract beast

`WitcherMonsters_ContractBeast` hediff (hidden label) + `HediffComp_ForceManhunter`:

- On add and every **120 ticks**, re-applies `ManhunterPermanent` if missing (covers Axii tame/calm edge cases)
- `ContractBeastUtility.IsContractBeast(Pawn)` for future quest code

Harmony patches (`Patch_BlockPacification.cs`):

- `WitcherBase.CompAbilityEffect_TameAnimal.CanApplyOn` — blocks Axii on contract beasts
- `CompAbilityEffect_StopManhunter.CanApplyOn` / `.Apply` — blocks ideology Animal Calm

Ships **0Harmony.dll** in `Assemblies/` (Lib.Harmony 2.3.3).

## Leshen tuning

| Knob | Location | Default |
|------|----------|---------|
| Combat power | `PawnKindDef` | 350 |
| Body size / health | `ThingDef.race` | 1.6 / 3.0 scale |
| Vine whip damage / AP | `CompProperties_AbilityVineWhip` | 20 / 0.35 |
| Vine whip cooldown | `AbilityDef` | 450 ticks |
| Wolf count | `CompProperties_AbilitySummonWolves` | 2–3 |
| Blink range | `AbilityDef` verb | 1.9–6.9 cells |
| Blink cooldown | `AbilityDef` | 600 ticks |

Summoned wolves get `ManhunterPermanent` only (not the contract hediff). AI skips summon when **≥2** manhunting timber wolves are already within 12 cells and no hostiles are nearby.

## Art (placeholder → custom)

Until PNGs exist, the leshen uses **vanilla bear** textures tinted green with `drawSize` **2.8**.

**Folder:** `Textures/Things/Pawn/Animal/Leshen/`

| File | Size |
|------|------|
| `Leshen_east.png` | 512×512 RGBA |
| `Leshen_north.png` | 512×512 |
| `Leshen_south.png` | 512×512 |
| `Leshen_west.png` | 512×512 |
| `Dessicated_Leshen_*.png` | 512×512 ×4 directions |

Optional ability icons (256×256): `Textures/UI/Abilities/Leshen_VineWhip.png`, etc.

Update `texPath` in `Defs/ThingDefs_Races/WitcherMonsters_Leshen.xml` when art lands.

## Anomaly extension

`Optional/Anomaly/README.md` — future `MayRequire="Ludeon.RimWorld.Anomaly"` defs. Empty `Defs/Anomaly/` reserved.

## Build

```bash
cd Source && ./build.sh
```

Output: `Assemblies/WitcherMonsters.dll` + `Assemblies/0Harmony.dll`.

Symlink to RimWorld `Mods/WitcherMonsters`. Enable Biotech, Witcher Base, Witcher Monster Quests.

## Test plan

1. Dev spawn leshen → immediately manhunting; Axii and Animal Calm fail
2. Vine whip damages hostiles at range; ignores colonists when leshen is not hostile to them (N/A in normal spawn — leshen is manhunter)
3. Summon wolves spawns manhunting timber wolves near leshen
4. Blink moves leshen over obstacles within range
5. Mod loads with Core + Biotech + Witcher Base only (no Anomaly)
