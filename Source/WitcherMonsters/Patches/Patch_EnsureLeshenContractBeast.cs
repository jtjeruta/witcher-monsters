using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace WitcherMonsters.Patches
{
    [HarmonyPatch]
    public static class Patch_EnsureLeshenContractBeastOnSpawn
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (MethodInfo method in AccessTools.GetDeclaredMethods(typeof(GenSpawn)))
            {
                if (method.Name != nameof(GenSpawn.Spawn))
                {
                    continue;
                }

                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length >= 3
                    && parameters[0].ParameterType == typeof(Thing)
                    && parameters[1].ParameterType == typeof(IntVec3)
                    && parameters[2].ParameterType == typeof(Map))
                {
                    yield return method;
                }
            }
        }

        public static void Postfix(Thing newThing)
        {
            if (newThing is Pawn pawn)
            {
                MonsterSpawnUtility.ApplyContractBeastState(pawn);
            }
        }
    }
}
