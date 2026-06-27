using RimWorld;
using RimWorld.Planet;
using Verse;

namespace WitcherMonsters
{
    public class GenStep_ContractScene : GenStep
    {
        public override int SeedPart => 9847123;

        public override void Generate(Map map, GenStepParams parms)
        {
            WorldObjectComp_ContractSiteData data = GetSiteData(map);
            if (data == null)
            {
                return;
            }

            MapComponent_ContractSite siteComp = EnsureSiteComp(map);
            ContractSceneType scene = data.sceneType;
            if (scene == ContractSceneType.Random)
            {
                scene = ContractEncounterUtility.PickScene(data.flavor);
            }

            ContractSceneGenerator.GenerateScene(map, scene);
            siteComp.Configure(data.encounter, scene);
        }

        internal static MapComponent_ContractSite EnsureSiteComp(Map map)
        {
            MapComponent_ContractSite comp = GetContractSiteComp(map);
            if (comp != null)
            {
                return comp;
            }

            comp = new MapComponent_ContractSite(map);
            map.components.Add(comp);
            return comp;
        }

        internal static MapComponent_ContractSite GetContractSiteComp(Map map)
        {
            if (map == null)
            {
                return null;
            }

            MapComponent_ContractSite comp = map.GetComponent<MapComponent_ContractSite>();
            if (comp != null)
            {
                return comp;
            }

            foreach (MapComponent component in map.components)
            {
                if (component is MapComponent_ContractSite siteComp)
                {
                    return siteComp;
                }
            }

            return null;
        }

        internal static WorldObjectComp_ContractSiteData GetSiteData(Map map)
        {
            Site site = Find.WorldObjects.WorldObjectAt<Site>(map.Tile);
            return site?.GetComponent<WorldObjectComp_ContractSiteData>();
        }
    }

    public class GenStep_ContractThreats : GenStep
    {
        public override int SeedPart => 9847124;

        public override void Generate(Map map, GenStepParams parms)
        {
            WorldObjectComp_ContractSiteData data = GenStep_ContractScene.GetSiteData(map);
            if (data?.encounter == null)
            {
                Log.Warning("[WitcherMonsters] Contract site missing encounter def.");
                return;
            }

            GenStep_ContractScene.EnsureSiteComp(map);
            ContractThreatSpawner.SpawnEncounter(map, data.encounter, map.Center);
        }
    }
}
