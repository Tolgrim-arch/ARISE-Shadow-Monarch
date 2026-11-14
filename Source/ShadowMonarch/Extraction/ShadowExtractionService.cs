using RimWorld;
using ShadowMonarch.Core;
using ShadowMonarch.Rendering;
using ShadowMonarch.Shadows;
using Verse;

namespace ShadowMonarch.Extraction;

public static class ShadowExtractionService
{
    public static bool CanExtract(Pawn monarch, Corpse? corpse, out string reason)
    {
        var registry = ShadowRegistryGameComponent.Current;
        if (registry == null || !registry.IsMonarch(monarch))
        {
            reason = "ShadowMonarch_NotMonarch".Translate();
            return false;
        }

        if (corpse == null || corpse.Destroyed)
        {
            reason = "ShadowMonarch_InvalidCorpse".Translate();
            return false;
        }

        var innerPawn = corpse.InnerPawn;
        if (innerPawn == null || !innerPawn.Dead || innerPawn.Destroyed)
        {
            reason = "ShadowMonarch_InvalidCorpse".Translate();
            return false;
        }

        if (registry.IsCorpseExtracted(corpse))
        {
            reason = "ShadowMonarch_AlreadyExtracted".Translate();
            return false;
        }

        if (!IsSupportedPawn(innerPawn))
        {
            reason = "ShadowMonarch_UnsupportedCreature".Translate(innerPawn.def.LabelCap);
            return false;
        }

        if (registry.GetRemainingAttempts(corpse) <= 0)
        {
            reason = "ShadowMonarch_NoAttemptsRemaining".Translate();
            return false;
        }

        reason = string.Empty;
        return true;
    }

    public static bool TryExtract(Pawn monarch, Corpse corpse, out string message)
    {
        if (!CanExtract(monarch, corpse, out var reason))
        {
            message = reason;
            return false;
        }

        var registry = ShadowRegistryGameComponent.Current!;
        var settings = ShadowMonarchMod.Settings;

        registry.RecordExtractionAttempt(corpse);
        var remaining = registry.GetRemainingAttempts(corpse);

        if (!Rand.Chance(settings.ExtractionChance))
        {
            message = "ShadowMonarch_ExtractionFailed".Translate(
                corpse.InnerPawn.LabelShortCap,
                remaining,
                settings.ExtractionChance.ToStringPercent());
            return false;
        }

        var sourcePawn = corpse.InnerPawn;
        var corpsePos = corpse.Position;
        var corpseMap = corpse.Map;
        registry.MarkCorpseExtracted(corpse);

        // Detach pawn from corpse without duplicating equipment or inventory.
        corpse.InnerPawn = null;
        if (settings.DestroyCorpseOnSuccess && !corpse.Destroyed)
        {
            corpse.Destroy(DestroyMode.Vanish);
        }

        ShadowVisuals.PlayTransientEffect(corpsePos, corpseMap);

        var record = new ShadowRecord(registry.MonarchId, sourcePawn);
        registry.AddRecord(record);

        message = "ShadowMonarch_ExtractionSucceeded".Translate(sourcePawn.LabelShortCap);
        return true;
    }

    private static bool IsSupportedPawn(Pawn pawn)
    {
        if (pawn.RaceProps.IsMechanoid)
        {
            return false;
        }

        if (pawn.RaceProps.Humanlike)
        {
            return true;
        }

        return ShadowMonarchMod.Settings.AllowAnimalExtraction && pawn.RaceProps.Animal;
    }
}
