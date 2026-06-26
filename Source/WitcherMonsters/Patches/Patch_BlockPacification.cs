using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace WitcherMonsters.Patches
{
    [HarmonyPatch]
    public static class Patch_BlockAxiiTame
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(
                AccessTools.TypeByName("WitcherBase.CompAbilityEffect_TameAnimal"),
                "CanApplyOn",
                new[] { typeof(LocalTargetInfo), typeof(LocalTargetInfo) });
        }

        private static bool Prefix(LocalTargetInfo target, ref bool __result)
        {
            if (ContractBeastUtility.IsContractBeast(target.Pawn))
            {
                __result = false;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(CompAbilityEffect_StopManhunter), nameof(CompAbilityEffect_StopManhunter.CanApplyOn))]
    public static class Patch_BlockStopManhunterCanApply
    {
        private static bool Prefix(LocalTargetInfo target, ref bool __result)
        {
            if (ContractBeastUtility.IsContractBeast(target.Pawn))
            {
                __result = false;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(CompAbilityEffect_StopManhunter), nameof(CompAbilityEffect_StopManhunter.Apply))]
    public static class Patch_BlockStopManhunterApply
    {
        private static bool Prefix(LocalTargetInfo target)
        {
            return !ContractBeastUtility.IsContractBeast(target.Pawn);
        }
    }
}
