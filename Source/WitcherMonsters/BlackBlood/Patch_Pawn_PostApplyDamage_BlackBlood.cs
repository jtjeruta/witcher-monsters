using Verse;

namespace WitcherMonsters
{
    internal static class Patch_Pawn_PostApplyDamage_BlackBlood
    {
        public static void Postfix(Pawn __instance, DamageInfo dinfo, float totalDamageDealt)
        {
            if (totalDamageDealt <= 0f || !BlackBloodUtility.IsEnabled || !BlackBloodUtility.HasBlackBlood(__instance))
            {
                return;
            }

            if (dinfo.Instigator is not Pawn attacker || attacker == __instance)
            {
                return;
            }

            if (!IsMeleeDamage(dinfo))
            {
                return;
            }

            BlackBloodContactPoison.ApplyLightDose(attacker);
        }

        private static bool IsMeleeDamage(DamageInfo dinfo)
        {
            if (dinfo.Weapon?.IsMeleeWeapon == true)
            {
                return true;
            }

            return dinfo.Tool != null;
        }
    }
}
