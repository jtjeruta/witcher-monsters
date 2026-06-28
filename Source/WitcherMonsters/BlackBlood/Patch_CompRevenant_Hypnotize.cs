using RimWorld;
using Verse;

namespace WitcherMonsters
{
    internal static class Patch_CompRevenant_Hypnotize
    {
        public static bool Prefix(CompRevenant __instance, Pawn victim)
        {
            if (!BlackBloodUtility.IsEnabled || !BlackBloodUtility.HasBlackBlood(victim))
            {
                return true;
            }

            BlackBloodContactPoison.ApplyRevenantBacklash(__instance, victim);

            Messages.Message(
                victim.LabelShort + "'s tainted blood repelled the revenant's hypnosis.",
                victim,
                MessageTypeDefOf.PositiveEvent);
            return false;
        }
    }
}
