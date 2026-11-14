using RimWorld;
using ShadowMonarch.Core;
using UnityEngine;
using Verse;

namespace ShadowMonarch.Rendering;

public static class ShadowVisuals
{
    public static void PlayTransientEffect(IntVec3 cell, Map? map)
    {
        if (!ShadowMonarchMod.Settings.EnableVisualEffects || map == null || !cell.InBounds(map))
        {
            return;
        }

        FleckMaker.ThrowSmoke(cell.ToVector3Shifted(), map, 1.5f);
        FleckMaker.Static(cell, map, FleckDefOf.PsycastAreaEffect, 1.2f);
    }
}
