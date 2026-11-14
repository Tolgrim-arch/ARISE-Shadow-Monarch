using RimWorld;
using ShadowMonarch.Shadows;
using UnityEngine;
using Verse;

namespace ShadowMonarch.UI;

public sealed class Dialog_ShadowArmy : Window
{
    private readonly Pawn _monarch;
    private Vector2 _scrollPosition;

    public override Vector2 InitialSize => new(560f, 440f);

    public Dialog_ShadowArmy(Pawn monarch)
    {
        _monarch = monarch;
        doCloseX = true;
        doCloseButton = true;
        draggable = true;
        absorbInputAroundWindow = false;
    }

    public override void DoWindowContents(Rect inRect)
    {
        var registry = ShadowRegistryGameComponent.Current;
        Text.Font = GameFont.Medium;
        Widgets.Label(new Rect(inRect.x, inRect.y, inRect.width, 32f), "ShadowMonarch_ArmyWindowTitle".Translate());
        Text.Font = GameFont.Small;

        if (registry == null || registry.Records.Count == 0)
        {
            Widgets.Label(new Rect(inRect.x, inRect.y + 44f, inRect.width, 30f), "ShadowMonarch_NoShadowsStored".Translate());
            return;
        }

        var listRect = new Rect(inRect.x, inRect.y + 40f, inRect.width, inRect.height - 90f);
        var viewRect = new Rect(0f, 0f, listRect.width - 16f, registry.Records.Count * 42f);

        Widgets.BeginScrollView(listRect, ref _scrollPosition, viewRect);
        var y = 0f;
        for (var i = 0; i < registry.Records.Count; i++)
        {
            var record = registry.Records[i];
            var rowRect = new Rect(0f, y, viewRect.width, 36f);
            if (i % 2 == 1)
            {
                Widgets.DrawLightHighlight(rowRect);
            }

            var labelRect = new Rect(rowRect.x + 6f, rowRect.y + 6f, rowRect.width - 140f, 24f);
            Widgets.Label(labelRect, $"{record.OriginalName} ({record.OriginalRaceDefName}) — {record.State}");

            var buttonRect = new Rect(rowRect.xMax - 126f, rowRect.y + 4f, 120f, 28f);
            if (record.State == ShadowState.Stored)
            {
                if (Widgets.ButtonText(buttonRect, "ShadowMonarch_SummonButton".Translate()))
                {
                    var ok = ShadowLifecycleService.TrySummon(record, _monarch, out var msg);
                    Messages.Message(msg, _monarch, ok ? MessageTypeDefOf.PositiveEvent : MessageTypeDefOf.RejectInput, false);
                }
            }
            else if (record.State == ShadowState.Summoned)
            {
                if (Widgets.ButtonText(buttonRect, "ShadowMonarch_RecallButton".Translate()))
                {
                    var ok = ShadowLifecycleService.TryRecall(record, out var msg);
                    Messages.Message(msg, _monarch, ok ? MessageTypeDefOf.NeutralEvent : MessageTypeDefOf.RejectInput, false);
                }
            }
            else
            {
                Widgets.Label(buttonRect, record.State.ToString());
            }

            y += 42f;
        }
        Widgets.EndScrollView();
    }
}
