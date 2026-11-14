using Verse;

namespace ShadowMonarch.Core;

public sealed class ShadowMonarchSettings : ModSettings
{
    public float ExtractionChance = 0.70f;
    public int MaxExtractionAttempts = 3;
    public int RecoveryHours = 6;
    public bool DestroyCorpseOnSuccess = true;
    public bool AllowAnimalExtraction = false;
    public bool EnableVisualEffects = true;

    public override void ExposeData()
    {
        Scribe_Values.Look(ref ExtractionChance, "extractionChance", 0.70f);
        Scribe_Values.Look(ref MaxExtractionAttempts, "maxExtractionAttempts", 3);
        Scribe_Values.Look(ref RecoveryHours, "recoveryHours", 6);
        Scribe_Values.Look(ref DestroyCorpseOnSuccess, "destroyCorpseOnSuccess", true);
        Scribe_Values.Look(ref AllowAnimalExtraction, "allowAnimalExtraction", false);
        Scribe_Values.Look(ref EnableVisualEffects, "enableVisualEffects", true);
        base.ExposeData();
    }
}
