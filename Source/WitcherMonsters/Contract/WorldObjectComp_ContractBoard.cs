using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace WitcherMonsters
{
    public class WorldObjectComp_ContractBoard : WorldObjectComp
    {
        private List<ContractOffer> offers = new List<ContractOffer>();
        private int nextRestockTick = -1;
        private bool everGenerated;

        public Settlement Settlement => parent as Settlement;

        public IReadOnlyList<ContractOffer> Offers => offers;

        public override void CompTickInterval(int delta)
        {
            base.CompTickInterval(delta);
            Settlement settlement = Settlement;
            if (settlement == null || !ContractBoardUtility.CanOfferContracts(settlement))
            {
                return;
            }

            if (!everGenerated)
            {
                return;
            }

            if (Find.TickManager.TicksGame >= nextRestockTick)
            {
                RegenerateOffers();
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref offers, "offers", LookMode.Deep);
            Scribe_Values.Look(ref nextRestockTick, "nextRestockTick", -1);
            Scribe_Values.Look(ref everGenerated, "everGenerated");

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                offers = offers ?? new List<ContractOffer>();
                PurgeInvalidOffers();
            }
        }

        public override IEnumerable<Gizmo> GetCaravanGizmos(Caravan caravan)
        {
            Settlement settlement = Settlement;
            if (settlement == null || caravan == null || caravan.Faction != Faction.OfPlayer)
            {
                yield break;
            }

            if (!ContractBoardUtility.CanOfferContracts(settlement))
            {
                yield break;
            }

            EnsureInitialized();

            yield return new Command_Action
            {
                defaultLabel = "Browse witcher contracts",
                defaultDesc = "Listen to frightened villagers describe monsters plaguing the roads.\n\n"
                    + offers.Count + " contract(s) posted.",
                icon = ContentFinder<Texture2D>.Get("UI/Commands/Trade", true),
                action = () => Find.WindowStack.Add(new Dialog_ContractBoard(caravan, settlement, this))
            };
        }

        public void EnsureInitialized()
        {
            if (!everGenerated)
            {
                RegenerateOffers();
                return;
            }

            PurgeInvalidOffers();
            if (offers.Count == 0)
            {
                RegenerateOffers();
            }
        }

        public void RegenerateOffers()
        {
            offers.Clear();
            int targetCount = Rand.RangeInclusive(2, 4);
            int attempts = 0;
            const int maxAttempts = 32;

            while (offers.Count < targetCount && attempts < maxAttempts)
            {
                attempts++;
                ContractOffer offer = ContractQuestUtility.RollOffer(Settlement);
                if (ContractEncounterUtility.IsOfferValid(offer))
                {
                    offers.Add(offer);
                }
            }

            PurgeInvalidOffers();
            everGenerated = true;
            nextRestockTick = Find.TickManager.TicksGame + 12 * GenDate.TicksPerDay;
        }

        private void PurgeInvalidOffers()
        {
            offers.RemoveAll(o => !ContractEncounterUtility.IsOfferValid(o));
        }

        public bool TryAccept(Caravan caravan, ContractOffer offer, out string failReason)
        {
            failReason = null;
            if (caravan == null || offer == null || !offers.Contains(offer))
            {
                failReason = "That contract is no longer available.";
                return false;
            }

            if (!ContractEncounterUtility.IsOfferValid(offer))
            {
                failReason = "That contract is no longer available.";
                offers.Remove(offer);
                return false;
            }

            if (!WitcherMonstersResearchUtility.IsWitcherTraditionsResearched())
            {
                failReason = "Your colony has not researched witcher traditions.";
                return false;
            }

            Map map = Find.AnyPlayerHomeMap;
            Pawn acceptor = caravan.PawnsListForReading.FirstOrDefault(p => p.IsColonist);
            Quest quest = ContractQuestUtility.StartContractHuntQuest(
                offer,
                Settlement,
                map,
                acceptImmediately: true,
                acceptor: acceptor);
            if (quest == null || quest.State != QuestState.Ongoing)
            {
                failReason = "Could not start contract.";
                return false;
            }

            offers.Remove(offer);
            Messages.Message(
                caravan.Name + " accepted a monster contract from " + Settlement.LabelCap + ".",
                caravan,
                MessageTypeDefOf.NeutralEvent);
            return true;
        }

        public string DescribeOffer(ContractOffer offer)
        {
            if (offer?.flavor == null)
            {
                return "Unavailable";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ContractEncounterUtility.ResolveQuestTitle(offer.flavor, Settlement));
            sb.AppendLine(ContractEncounterUtility.ResolveQuestBody(offer.flavor, Settlement));
            sb.AppendLine();
            sb.Append("Reward: " + ContractEncounterUtility.RewardLabel(offer.rewardType));
            return sb.ToString();
        }
    }
}
