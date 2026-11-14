using RimWorld;
using ShadowMonarch.Core;
using Verse;
using Verse.AI;

namespace ShadowMonarch.Shadows;

public static class ShadowLifecycleService
{
    public static bool TrySummon(ShadowRecord record, Pawn monarch, out string message)
    {
        if (record == null || record.Pawn == null)
        {
            message = "ShadowMonarch_InvalidShadowRecord".Translate();
            return false;
        }

        if (record.State != ShadowState.Stored)
        {
            message = "ShadowMonarch_ShadowNotStored".Translate(record.OriginalName, record.State.ToString());
            return false;
        }

        if (monarch == null || !monarch.Spawned || monarch.Map == null)
        {
            message = "ShadowMonarch_MonarchNotOnMap".Translate();
            return false;
        }

        var pawn = record.Pawn;
        if (pawn.Spawned)
        {
            record.State = ShadowState.Summoned;
            message = "ShadowMonarch_AlreadySummoned".Translate(record.OriginalName);
            return false;
        }

        var map = monarch.Map;
        if (!CellFinder.TryFindRandomCellNear(
                monarch.Position,
                map,
                4,
                c => c.InBounds(map)
                     && c.Standable(map)
                     && !c.Fogged(map)
                     && map.reachability.CanReach(monarch.Position, c, PathEndMode.OnCell, TraverseParms.For(monarch)),
                out var spawnCell))
        {
            message = "ShadowMonarch_NoValidSummonCell".Translate();
            return false;
        }

        if (pawn.Dead)
        {
            ResurrectionUtility.TryResurrect(pawn);
        }

        if (pawn.Faction != Faction.OfPlayer)
        {
            pawn.SetFaction(Faction.OfPlayer);
        }

        GenSpawn.Spawn(pawn, spawnCell, map, WipeMode.Vanish);
        record.State = ShadowState.Summoned;

        message = "ShadowMonarch_SummonSucceeded".Translate(record.OriginalName);
        return true;
    }

    public static bool TryRecall(ShadowRecord record, out string message)
    {
        if (record == null || record.Pawn == null)
        {
            message = "ShadowMonarch_InvalidShadowRecord".Translate();
            return false;
        }

        if (record.State != ShadowState.Summoned)
        {
            message = "ShadowMonarch_ShadowNotSummoned".Translate(record.OriginalName);
            return false;
        }

        var pawn = record.Pawn;
        if (pawn.Spawned)
        {
            pawn.DeSpawn(DestroyMode.Vanish);
        }

        record.State = ShadowState.Stored;
        message = "ShadowMonarch_RecallSucceeded".Translate(record.OriginalName);
        return true;
    }

    public static void HandleDefeat(ShadowRecord record)
    {
        if (record == null || record.State == ShadowState.Recovering)
        {
            return;
        }

        var pawn = record.Pawn;
        if (pawn != null && pawn.Spawned)
        {
            pawn.DeSpawn(DestroyMode.Vanish);
        }

        var recoveryTicks = ShadowMonarchMod.Settings.RecoveryHours * GenDate.TicksPerHour;
        record.RecoveryCompleteTick = Find.TickManager.TicksGame + recoveryTicks;
        record.State = ShadowState.Recovering;

        Messages.Message(
            "ShadowMonarch_ShadowDefeated".Translate(record.OriginalName, ShadowMonarchMod.Settings.RecoveryHours),
            MessageTypeDefOf.NeutralEvent,
            false);
    }

    public static void CompleteRecovery(ShadowRecord record)
    {
        if (record == null || record.State != ShadowState.Recovering)
        {
            return;
        }

        var pawn = record.Pawn;
        if (pawn != null)
        {
            if (pawn.Dead)
            {
                ResurrectionUtility.TryResurrect(pawn);
            }

            pawn.health?.hediffSet?.hediffs?.RemoveAll(h => h is Hediff_Injury);
            pawn.health?.Notify_HediffChanged(null);
        }

        record.RecoveryCompleteTick = -1;
        record.State = ShadowState.Stored;

        Messages.Message(
            "ShadowMonarch_ShadowRecovered".Translate(record.OriginalName),
            MessageTypeDefOf.PositiveEvent,
            false);
    }
}
