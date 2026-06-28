using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

class Program
{
    static void Main()
    {
        var harmony = new Harmony("test");
        foreach (var (type, method) in new[]
        {
            (typeof(CompRevenant), "EndHypnotize"),
            (typeof(Pawn), nameof(Pawn.PostApplyDamage)),
        })
        {
            var m = AccessTools.Method(type, method);
            Console.WriteLine(m != null ? "OK: " + m : "MISSING: " + type.Name + "." + method);
        }
    }
}
