using HarmonyLib;
using ShadowMonarch.Shadows;
using Verse;

namespace ShadowMonarch.Patches;

[HarmonyPatch(typeof(Pawn), nameof(Pawn.Kill))]
public static class ShadowImmortality_Patch
{
    public static bool Prefix(Pawn __instance)
    {
        var registry = ShadowRegistryGameComponent.Current;
        if (registry == null || !registry.TryGetRecord(__instance, out var record) || record == null)
        {
            return true;
        }

        if (record.State != ShadowState.Summoned)
        {
            return true;
        }

        ShadowLifecycleService.HandleDefeat(record);
        return false;
    }
}
