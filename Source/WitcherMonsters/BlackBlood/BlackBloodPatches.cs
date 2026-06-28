using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace WitcherMonsters
{
    [StaticConstructorOnStartup]
    internal static class BlackBloodPatches
    {
        private const string HarmonyId = "witcher.monsters.blackblood";

        static BlackBloodPatches()
        {
            if (!BlackBloodUtility.IsEnabled)
            {
                return;
            }

            Harmony harmony = new Harmony(HarmonyId);
            TryPatch(
                harmony,
                () =>
                {
                    MethodInfo target = AccessTools.Method(typeof(CompRevenant), nameof(CompRevenant.Hypnotize));
                    if (target == null)
                    {
                        Log.Warning("[WitcherMonsters] Black blood patch skipped: CompRevenant.Hypnotize not found.");
                        return;
                    }

                    harmony.Patch(
                        target,
                        prefix: new HarmonyMethod(typeof(Patch_CompRevenant_Hypnotize), nameof(Patch_CompRevenant_Hypnotize.Prefix)));
                });
            TryPatch(
                harmony,
                () =>
                {
                    MethodInfo target = AccessTools.Method(typeof(Pawn), nameof(Pawn.PostApplyDamage));
                    if (target == null)
                    {
                        Log.Warning("[WitcherMonsters] Black blood patch skipped: Pawn.PostApplyDamage not found.");
                        return;
                    }

                    harmony.Patch(
                        target,
                        postfix: new HarmonyMethod(typeof(Patch_Pawn_PostApplyDamage_BlackBlood), nameof(Patch_Pawn_PostApplyDamage_BlackBlood.Postfix)));
                });
        }

        private static void TryPatch(Harmony harmony, Action patchAction)
        {
            try
            {
                patchAction();
            }
            catch (Exception ex)
            {
                Log.Warning("[WitcherMonsters] Black blood patch failed: " + ex);
            }
        }
    }
}
