using HarmonyLib;
using Verse;

namespace WitcherMonsters
{
    public class WitcherMonstersMod : Mod
    {
        public WitcherMonstersMod(ModContentPack content) : base(content)
        {
            new Harmony("witcher.monsters").PatchAll(typeof(WitcherMonstersMod).Assembly);
        }
    }
}
