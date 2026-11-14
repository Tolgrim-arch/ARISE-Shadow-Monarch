using HarmonyLib;
using RimWorld;
using ShadowMonarch.Shadows;
using Verse;
using Verse.AI;

namespace ShadowMonarch.Patches;

[HarmonyPatch(typeof(Need), "IsFrozen", MethodType.Getter)]
public static class ShadowNeedFrozen_Patch
{
    public static bool Prefix(Pawn ___pawn, ref bool __result)
    {
        var registry = ShadowRegistryGameComponent.Current;
        if (registry != null && registry.IsShadow(___pawn))
        {
            __result = true;
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(MentalStateHandler), nameof(MentalStateHandler.TryStartMentalState))]
public static class ShadowMentalState_Patch
{
    public static bool Prefix(Pawn ___pawn)
    {
        var registry = ShadowRegistryGameComponent.Current;
        return registry == null || !registry.IsShadow(___pawn);
    }
}
