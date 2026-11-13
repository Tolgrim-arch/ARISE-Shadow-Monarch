using LudeonTK;
using RimWorld;
using ShadowMonarch.Shadows;
using Verse;

namespace ShadowMonarch.Monarch;

public static class MonarchDebugActions
{
    [DebugAction("ARISE: Shadow Monarch", "Designate Monarch", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    private static void DesignateMonarch(Pawn p)
    {
        var registry = ShadowRegistryGameComponent.Current;
        if (registry == null)
        {
            return;
        }

        if (registry.DesignateMonarch(p, out var reason))
        {
            Messages.Message("ShadowMonarch_MonarchDesignated".Translate(p.LabelShortCap), p, MessageTypeDefOf.PositiveEvent, false);
        }
        else
        {
            Messages.Message(reason, p, MessageTypeDefOf.RejectInput, false);
        }
    }

    [DebugAction("ARISE: Shadow Monarch", "Log Registry State", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.Playing)]
    private static void LogRegistryState()
    {
        var registry = ShadowRegistryGameComponent.Current;
        if (registry == null)
        {
            Log.Message("[ARISE] ShadowRegistryGameComponent is null.");
            return;
        }

        var monarchName = registry.MonarchPawn?.LabelShortCap ?? "None";
        Log.Message($"[ARISE] Monarch: {monarchName} ({registry.MonarchId}), Shadows: {registry.Records.Count}");
    }
}
