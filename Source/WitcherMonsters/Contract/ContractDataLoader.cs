using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Verse;

namespace WitcherMonsters
{
    [StaticConstructorOnStartup]
    public static class ContractDataLoader
    {
        private static readonly List<ContractFlavorDef> flavors = new List<ContractFlavorDef>();
        private static readonly List<ContractEncounterDef> encounters = new List<ContractEncounterDef>();
        private static Dictionary<string, ContractFlavorDef> flavorByName;
        private static Dictionary<string, ContractEncounterDef> encounterByName;

        public static IReadOnlyList<ContractEncounterDef> AllEncounters => encounters;

        static ContractDataLoader()
        {
            ModContentPack pack = LoadedModManager.RunningModsListForReading
                ?.FirstOrDefault(m => m.PackageId == "witcher.monsters");
            if (pack == null)
            {
                Log.Warning("[WitcherMonsters] Could not find mod content pack for contract data.");
                return;
            }

            string dataDir = Path.Combine(pack.RootDir, "ContractData");
            if (!Directory.Exists(dataDir))
            {
                Log.Warning("[WitcherMonsters] ContractData folder missing at " + dataDir);
                return;
            }

            foreach (string file in Directory.GetFiles(dataDir, "*.xml", SearchOption.AllDirectories))
            {
                LoadFile(file);
            }

            flavorByName = flavors
                .Where(f => !f.defName.NullOrEmpty())
                .ToDictionary(f => f.defName);
            encounterByName = encounters
                .Where(e => !e.defName.NullOrEmpty())
                .ToDictionary(e => e.defName);

            int loadedEncounters = encounters.Count;
            int vanBeastsLoaded = encounters.Count(e => e.requiredModPackageId == "Van.Beasts");

            Log.Message("[WitcherMonsters] Loaded " + loadedEncounters + " contract encounter defs ("
                + vanBeastsLoaded + " Van.Beasts) and " + flavors.Count + " flavor variants.");
        }

        private static void LoadFile(string path)
        {
            XmlDocument document = new XmlDocument();
            document.Load(path);
            XmlNode root = document.DocumentElement;
            if (root == null)
            {
                return;
            }

            foreach (XmlNode node in root.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                if (!PassesMayRequire(node))
                {
                    continue;
                }

                if (node.Name == "ContractFlavorDef")
                {
                    flavors.Add(DirectXmlToObject.ObjectFromXml<ContractFlavorDef>(node, false));
                }
                else if (node.Name == "ContractEncounterDef")
                {
                    encounters.Add(DirectXmlToObject.ObjectFromXml<ContractEncounterDef>(node, false));
                }
            }
        }

        private static bool PassesMayRequire(XmlNode node)
        {
            XmlAttribute mayRequire = node.Attributes?["MayRequire"];
            if (mayRequire == null || mayRequire.Value.NullOrEmpty())
            {
                return true;
            }

            return ModsConfig.IsActive(mayRequire.Value);
        }

        public static ContractEncounterDef GetEncounter(string defName)
        {
            if (defName.NullOrEmpty() || encounterByName == null)
            {
                return null;
            }

            encounterByName.TryGetValue(defName, out ContractEncounterDef encounter);
            return encounter;
        }

        public static ContractFlavorDef GetFlavor(string defName)
        {
            if (defName.NullOrEmpty() || flavorByName == null)
            {
                return null;
            }

            flavorByName.TryGetValue(defName, out ContractFlavorDef flavor);
            return flavor;
        }
    }
}
