using HarmonyLib;
using RimWorld;
using ShadowMonarch.Shadows;
using Verse;
using Verse.AI;

namespace ShadowMonarch.Patches;

[HarmonyPatch(typeof(Need), nameof(Need.NeedInterval))]
public static class ShadowNeedInterval_Patch
{
    public static bool Prefix(Pawn ___pawn)
    {
        var registry = ShadowRegistryGameComponent.Current;
        return registry == null || !registry.IsShadow(___pawn);
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
