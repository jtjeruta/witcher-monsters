using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace WitcherMonsters
{
    internal static class DrownerLungeUtility
    {
        internal static void ApplyLungeDamage(Pawn caster, LocalTargetInfo target)
        {
            if (caster == null || !target.HasThing || !(target.Thing is Pawn victim) || victim.Dead || victim == caster)
            {
                return;
            }

            DamageInfo bite = new DamageInfo(DamageDefOf.Bite, Rand.Range(8f, 14f), 0.15f, -1f, caster);
            victim.TakeDamage(bite);

            DamageInfo claw = new DamageInfo(DamageDefOf.Scratch, Rand.Range(4f, 8f), 0.1f, -1f, caster);
            victim.TakeDamage(claw);
        }
    }

    [StaticConstructorOnStartup]
    internal static class DrownerLungePatches
    {
        static DrownerLungePatches()
        {
            Harmony harmony = new Harmony("witcher.monsters.drowner");
            System.Reflection.MethodInfo target = AccessTools.Method(typeof(CompAbilityEffect_ConsumeLeap), "OnJumpCompleted");
            if (target == null)
            {
                Log.Warning("[WitcherMonsters] Drowner lunge patch skipped: CompAbilityEffect_ConsumeLeap.OnJumpCompleted not found.");
                return;
            }

            harmony.Patch(
                target,
                prefix: new HarmonyMethod(typeof(DrownerLungePatches), nameof(OnJumpCompletedPrefix)));
        }

        private static bool OnJumpCompletedPrefix(CompAbilityEffect_ConsumeLeap __instance, IntVec3 origin, LocalTargetInfo target)
        {
            if (!(__instance is CompAbilityEffect_DrownerLunge))
            {
                return true;
            }

            DrownerLungeUtility.ApplyLungeDamage(__instance.parent.pawn, target);
            return false;
        }
    }
}
