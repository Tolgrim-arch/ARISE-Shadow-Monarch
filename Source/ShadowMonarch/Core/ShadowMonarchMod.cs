using HarmonyLib;
using UnityEngine;
using Verse;

namespace ShadowMonarch.Core;

public sealed class ShadowMonarchMod : Mod
{
    public const string ModId = "tolgrim.ariseshadowmonarch";

    public static ShadowMonarchSettings Settings { get; private set; } = null!;

    public ShadowMonarchMod(ModContentPack content) : base(content)
    {
        Settings = GetSettings<ShadowMonarchSettings>();
        var harmony = new Harmony(ModId);
        harmony.PatchAll();
    }

    public override string SettingsCategory() => "ShadowMonarch_SettingsCategory".Translate();

    public override void DoSettingsWindowContents(Rect inRect)
    {
        var listing = new Listing_Standard();
        listing.Begin(inRect);

        listing.Label("ShadowMonarch_ExtractionChanceLabel".Translate(Settings.ExtractionChance.ToStringPercent()));
        Settings.ExtractionChance = listing.Slider(Settings.ExtractionChance, 0.05f, 1.0f);

        listing.Label("ShadowMonarch_MaxAttemptsLabel".Translate(Settings.MaxExtractionAttempts));
        Settings.MaxExtractionAttempts = Mathf.RoundToInt(listing.Slider(Settings.MaxExtractionAttempts, 1f, 10f));

        listing.Label("ShadowMonarch_RecoveryHoursLabel".Translate(Settings.RecoveryHours));
        Settings.RecoveryHours = Mathf.RoundToInt(listing.Slider(Settings.RecoveryHours, 1f, 72f));

        listing.GapLine();

        listing.CheckboxLabeled(
            "ShadowMonarch_DestroyCorpseOnSuccessLabel".Translate(),
            ref Settings.DestroyCorpseOnSuccess,
            "ShadowMonarch_DestroyCorpseOnSuccessTooltip".Translate());

        listing.CheckboxLabeled(
            "ShadowMonarch_AllowAnimalExtractionLabel".Translate(),
            ref Settings.AllowAnimalExtraction,
            "ShadowMonarch_AllowAnimalExtractionTooltip".Translate());

        listing.CheckboxLabeled(
            "ShadowMonarch_EnableVisualEffectsLabel".Translate(),
            ref Settings.EnableVisualEffects,
            "ShadowMonarch_EnableVisualEffectsTooltip".Translate());

        listing.GapLine();

        if (listing.ButtonText("ShadowMonarch_ResetDefaults".Translate()))
        {
            Settings.ExtractionChance = 0.70f;
            Settings.MaxExtractionAttempts = 3;
            Settings.RecoveryHours = 6;
            Settings.DestroyCorpseOnSuccess = true;
            Settings.AllowAnimalExtraction = false;
            Settings.EnableVisualEffects = true;
        }

        listing.End();
        base.DoSettingsWindowContents(inRect);
    }
}
