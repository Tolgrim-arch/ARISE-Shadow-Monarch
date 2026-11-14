using HarmonyLib;
using ShadowMonarch.Core;
using ShadowMonarch.Shadows;
using UnityEngine;
using Verse;

namespace ShadowMonarch.Patches;

[HarmonyPatch(typeof(PawnRenderNode), nameof(PawnRenderNode.ColorFor))]
public static class ShadowVisuals_Patch
{
    private static readonly Color ShadowMultiplyTint = new(0.32f, 0.14f, 0.48f, 1f);

    public static void Postfix(Pawn pawn, ref Color __result)
    {
        if (!ShadowMonarchMod.Settings.EnableVisualEffects)
        {
            return;
        }

        var registry = ShadowRegistryGameComponent.Current;
        if (registry != null && registry.IsShadow(pawn))
        {
            __result *= ShadowMultiplyTint;
        }
    }
}
