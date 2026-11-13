using System;
using Verse;

namespace ShadowMonarch.Shadows;

public sealed class ShadowRecord : IExposable
{
    public const int CurrentSchemaVersion = 1;

    public int SchemaVersion = CurrentSchemaVersion;
    public string ShadowId = string.Empty;
    public string OwnerMonarchId = string.Empty;
    public string OriginalName = string.Empty;
    public string OriginalRaceDefName = string.Empty;
    public ShadowState State = ShadowState.Stored;
    public int RecoveryCompleteTick = -1;

    // Never deep-save the same Pawn twice:
    // - Summoned pawns are owned by the Map/World and saved via Scribe_References.
    // - Stored/Recovering/Unavailable pawns are owned by the registry and saved via Scribe_Deep.
    private Pawn? _pawn;

    public Pawn? Pawn
    {
        get => _pawn;
        set => _pawn = value;
    }

    public ShadowRecord()
    {
    }

    public ShadowRecord(string ownerMonarchId, Pawn sourcePawn)
    {
        ShadowId = Guid.NewGuid().ToString("N");
        OwnerMonarchId = ownerMonarchId;
        _pawn = sourcePawn;
        OriginalName = sourcePawn.LabelShortCap;
        OriginalRaceDefName = sourcePawn.def.defName;
        State = ShadowState.Stored;
    }

    public void ExposeData()
    {
        Scribe_Values.Look(ref SchemaVersion, "schemaVersion", CurrentSchemaVersion);
        Scribe_Values.Look(ref ShadowId, "shadowId", string.Empty);
        Scribe_Values.Look(ref OwnerMonarchId, "ownerMonarchId", string.Empty);
        Scribe_Values.Look(ref OriginalName, "originalName", string.Empty);
        Scribe_Values.Look(ref OriginalRaceDefName, "originalRaceDefName", string.Empty);
        Scribe_Values.Look(ref State, "state", ShadowState.Stored);
        Scribe_Values.Look(ref RecoveryCompleteTick, "recoveryCompleteTick", -1);

        if (State == ShadowState.Summoned)
        {
            Scribe_References.Look(ref _pawn, "pawnRef");
        }
        else
        {
            Scribe_Deep.Look(ref _pawn, "pawnDeep");
        }
    }
}
