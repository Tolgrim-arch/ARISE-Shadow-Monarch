using HarmonyLib;
using ShadowMonarch.Shadows;
using Verse;

namespace ShadowMonarch.Patches;

[HarmonyPatch(typeof(Pawn), nameof(Pawn.Kill))]
public static class MonarchDeath_Patch
{
    public static void Postfix(Pawn __instance)
    {
        var registry = ShadowRegistryGameComponent.Current;
        if (registry == null || registry.MonarchPawn != __instance)
        {
            return;
        }

        for (var i = 0; i < registry.Records.Count; i++)
        {
            var record = registry.Records[i];
            if (record.State == ShadowState.Summoned && record.Pawn != null && record.Pawn.Spawned)
            {
                record.Pawn.DeSpawn(DestroyMode.Vanish);
            }
        }

        registry.MarkArmyUnavailable();
    }
}
