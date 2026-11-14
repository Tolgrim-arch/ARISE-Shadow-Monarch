using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using ShadowMonarch.Core;
using ShadowMonarch.Extraction;
using ShadowMonarch.Shadows;
using UnityEngine;
using Verse;

namespace ShadowMonarch.Patches;

[HarmonyPatch(typeof(Pawn), nameof(Pawn.GetGizmos))]
public static class PawnGizmos_Patch
{
    public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> gizmos, Pawn __instance)
    {
        foreach (var gizmo in gizmos)
        {
            yield return gizmo;
        }

        var registry = ShadowRegistryGameComponent.Current;
        if (registry == null || !registry.IsMonarch(__instance) || !__instance.Spawned)
        {
            yield break;
        }

        yield return new Command_Target
        {
            defaultLabel = "ShadowMonarch_AriseCommandLabel".Translate(),
            defaultDesc = "ShadowMonarch_AriseCommandDesc".Translate(
                ShadowMonarchMod.Settings.ExtractionChance.ToStringPercent(),
                ShadowMonarchMod.Settings.MaxExtractionAttempts),
            icon = TexCommand.Attack,
            targetingParams = new TargetingParameters
            {
                canTargetPawns = false,
                canTargetBuildings = false,
                canTargetItems = true,
                mapObjectTargetsMustBeAutoAttackable = false,
                validator = target => target.Thing is Corpse
            },
            action = target =>
            {
                if (target.Thing is not Corpse corpse)
                {
                    return;
                }

                var success = ShadowExtractionService.TryExtract(__instance, corpse, out var message);
                var msgType = success ? MessageTypeDefOf.PositiveEvent : MessageTypeDefOf.RejectInput;
                Messages.Message(message, corpse.Spawned ? corpse : __instance, msgType, false);
            }
        };
    }
}
