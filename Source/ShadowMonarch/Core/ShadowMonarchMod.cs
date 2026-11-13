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

        listing.Label("ShadowMonarch_RecoveryHoursLabel".Translate(Settings.RecoveryHours));
        Settings.RecoveryHours = Mathf.RoundToInt(listing.Slider(Settings.RecoveryHours, 1f, 72f));

        listing.End();
        base.DoSettingsWindowContents(inRect);
    }
}
