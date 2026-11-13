using Verse;

namespace ShadowMonarch.Core;

public sealed class ShadowMonarchSettings : ModSettings
{
    public float ExtractionChance = 0.70f;
    public int RecoveryHours = 6;

    public override void ExposeData()
    {
        Scribe_Values.Look(ref ExtractionChance, "extractionChance", 0.70f);
        Scribe_Values.Look(ref RecoveryHours, "recoveryHours", 6);
        base.ExposeData();
    }
}
