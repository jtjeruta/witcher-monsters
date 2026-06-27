using RimWorld;
using Verse;

namespace WitcherMonsters
{
    public static class ContractRewardUtility
    {
        public static void GrantContractReward(
            Map map,
            ContractRewardType rewardType,
            int rewardSilver,
            int rewardGoodwill,
            Faction rewardFaction)
        {
            switch (rewardType)
            {
                case ContractRewardType.Silver:
                    GiveSilver(map, rewardSilver);
                    break;
                case ContractRewardType.Goodwill:
                    GiveGoodwill(rewardFaction, rewardGoodwill);
                    break;
                case ContractRewardType.LawOfSurprise:
                    GameComponent_ContractSurprise.Schedule(map, rewardFaction, 7 * GenDate.TicksPerDay);
                    Messages.Message(
                        "The villagers swear the Law of Surprise — a debt will arrive within a week.",
                        MessageTypeDefOf.NeutralEvent);
                    break;
            }
        }

        public static void GiveSilver(Map map, int amount)
        {
            if (map == null || amount <= 0)
            {
                return;
            }

            Thing silver = ThingMaker.MakeThing(ThingDefOf.Silver);
            silver.stackCount = amount;
            DropPodUtility.DropThingsNear(map.Center, map, Gen.YieldSingle(silver), canInstaDropDuringInit: false);
            Messages.Message(
                "Contract reward delivered: " + amount + " silver.",
                new TargetInfo(map.Center, map),
                MessageTypeDefOf.PositiveEvent);
        }

        public static void GiveGoodwill(Faction faction, int amount)
        {
            if (faction == null || amount <= 0)
            {
                return;
            }

            faction.TryAffectGoodwillWith(Faction.OfPlayer, amount, canSendMessage: true);
        }
    }
}
