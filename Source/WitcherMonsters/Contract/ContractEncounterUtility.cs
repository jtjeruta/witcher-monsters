using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace WitcherMonsters
{
    public static class ContractEncounterUtility
    {
        public static IEnumerable<ContractEncounterDef> AllEncounters =>
            ContractDataLoader.AllEncounters;

        public static bool IsEncounterAvailable(ContractEncounterDef encounter)
        {
            if (encounter == null)
            {
                return false;
            }

            if (!encounter.requiredModPackageId.NullOrEmpty()
                && !ModsConfig.IsActive(encounter.requiredModPackageId))
            {
                return false;
            }

            if (encounter.spawnEntries.NullOrEmpty())
            {
                return false;
            }

            foreach (ContractSpawnEntry entry in encounter.spawnEntries)
            {
                if (entry?.PawnKindDef == null)
                {
                    return false;
                }
            }

            return encounter.flavorDefs != null && encounter.flavorDefs.Count > 0;
        }

        public static bool IsOfferValid(ContractOffer offer)
        {
            return offer != null
                && offer.flavor != null
                && IsEncounterAvailable(offer.encounter);
        }

        public static List<ContractEncounterDef> GetAvailableEncounters()
        {
            float wealth = GetPlayerWealthSafe();
            return AllEncounters
                .Where(e => IsEncounterAvailable(e) && wealth >= e.minWealth)
                .ToList();
        }

        public static int CountEncountersPassingModAndSpawnChecks()
        {
            return AllEncounters.Count(IsEncounterAvailable);
        }

        private static float GetPlayerWealthSafe()
        {
            if (Current.ProgramState != ProgramState.Playing)
            {
                return 0f;
            }

            if (Find.World == null || Find.AnyPlayerHomeMap == null)
            {
                return 0f;
            }

            return WealthUtility.PlayerWealth;
        }

        public static ContractEncounterDef PickRandomEncounter()
        {
            List<ContractEncounterDef> available = GetAvailableEncounters();
            if (available.Count == 0)
            {
                return null;
            }

            return available.RandomElementByWeight(e => e.selectionWeight);
        }

        public static ContractFlavorDef PickFlavor(ContractEncounterDef encounter)
        {
            if (encounter?.flavorDefs == null || encounter.flavorDefs.Count == 0)
            {
                return null;
            }

            string flavorName = encounter.flavorDefs.RandomElement();
            return ContractDataLoader.GetFlavor(flavorName);
        }

        public static ContractSceneType PickScene(ContractFlavorDef flavor)
        {
            if (flavor != null && flavor.preferredScene != ContractSceneType.Random)
            {
                return flavor.preferredScene;
            }

            return Rand.Element(
                ContractSceneType.AbandonedCart,
                ContractSceneType.AbandonedCamp,
                ContractSceneType.CorpseField);
        }

        public static ContractRewardType PickRewardType()
        {
            return Rand.Element(
                ContractRewardType.Silver,
                ContractRewardType.Goodwill,
                ContractRewardType.LawOfSurprise);
        }

        public static string RewardLabel(ContractRewardType rewardType)
        {
            switch (rewardType)
            {
                case ContractRewardType.Silver:
                    return "Silver payment";
                case ContractRewardType.Goodwill:
                    return "Faction favor";
                default:
                    return "Law of Surprise";
            }
        }

        public static string ResolveQuestTitle(ContractFlavorDef flavor, Settlement origin)
        {
            string title = flavor?.questTitle ?? "Monster contract";
            if (origin != null)
            {
                title = title.Replace("[settlement]", origin.LabelCap);
            }

            return title;
        }

        public static string ResolveQuestBody(ContractFlavorDef flavor, Settlement origin)
        {
            string body = flavor?.questDescription ?? "Villagers beg for help with a monster.";
            if (origin != null)
            {
                body = body.Replace("[settlement]", origin.LabelCap);
                body = body.Replace("[faction]", origin.Faction?.Name ?? "the settlement");
            }

            return body;
        }

        public static string ResolveQuestDescription(
            ContractFlavorDef flavor,
            Settlement origin,
            ContractRewardType rewardType)
        {
            return ResolveQuestBody(flavor, origin) + "\n\nReward: " + RewardLabel(rewardType) + ".";
        }

        public static Faction ResolveRewardFaction(Settlement origin)
        {
            if (origin?.Faction != null && !origin.Faction.IsPlayer)
            {
                return origin.Faction;
            }

            return Find.FactionManager.AllFactionsListForReading
                .FirstOrDefault(f => f != null && !f.IsPlayer && !f.HostileTo(Faction.OfPlayer));
        }
    }
}
