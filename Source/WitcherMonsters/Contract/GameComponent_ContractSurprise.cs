using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace WitcherMonsters
{
    public class GameComponent_ContractSurprise : GameComponent
    {
        private List<PendingSurprise> pending = new List<PendingSurprise>();

        public GameComponent_ContractSurprise(Game game)
        {
        }

        public static void Schedule(Map map, Faction faction, int delayTicks)
        {
            GameComponent_ContractSurprise comp = Current.Game?.GetComponent<GameComponent_ContractSurprise>();
            if (comp == null)
            {
                return;
            }

            comp.pending.Add(new PendingSurprise
            {
                triggerTick = Find.TickManager.TicksGame + delayTicks,
                map = map,
                faction = faction
            });
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();
            int now = Find.TickManager.TicksGame;
            for (int i = pending.Count - 1; i >= 0; i--)
            {
                PendingSurprise entry = pending[i];
                if (entry == null || now < entry.triggerTick)
                {
                    continue;
                }

                Map targetMap = entry.map ?? Find.AnyPlayerHomeMap;
                if (targetMap != null)
                {
                    LawOfSurpriseUtility.DeliverRandomSurprise(targetMap, entry.faction);
                }

                pending.RemoveAt(i);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref pending, "pending", LookMode.Deep);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                pending = pending ?? new List<PendingSurprise>();
            }
        }

        private class PendingSurprise : IExposable
        {
            public int triggerTick;
            public Map map;
            public Faction faction;

            public void ExposeData()
            {
                Scribe_Values.Look(ref triggerTick, "triggerTick");
                Scribe_References.Look(ref map, "map");
                Scribe_References.Look(ref faction, "faction");
            }
        }
    }
}
