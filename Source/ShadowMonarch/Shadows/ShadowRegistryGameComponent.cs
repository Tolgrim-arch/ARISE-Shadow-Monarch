using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ShadowMonarch.Shadows;

public sealed class ShadowRegistryGameComponent : GameComponent
{
    public const int CurrentSchemaVersion = 1;

    private int _schemaVersion = CurrentSchemaVersion;
    private Pawn? _monarchPawn;
    private string _monarchId = string.Empty;
    private List<ShadowRecord> _records = new();
    private Dictionary<string, int> _corpseExtractionAttempts = new();

    public Pawn? MonarchPawn => _monarchPawn;
    public string MonarchId => _monarchId;
    public IReadOnlyList<ShadowRecord> Records => _records;

    public ShadowRegistryGameComponent(Game game)
    {
    }

    public static ShadowRegistryGameComponent? Current => Verse.Current.Game?.GetComponent<ShadowRegistryGameComponent>();

    public bool IsMonarch(Pawn? pawn)
    {
        return pawn != null && _monarchPawn == pawn && !pawn.Dead && !pawn.Destroyed;
    }

    public bool DesignateMonarch(Pawn pawn, out string reason)
    {
        if (pawn == null || pawn.Dead || pawn.Destroyed)
        {
            reason = "ShadowMonarch_InvalidMonarchTarget".Translate();
            return false;
        }

        if (pawn.Faction != Faction.OfPlayer)
        {
            reason = "ShadowMonarch_MonarchMustBePlayerFaction".Translate();
            return false;
        }

        _monarchPawn = pawn;
        _monarchId = pawn.GetUniqueLoadID();
        reason = string.Empty;
        return true;
    }

    public int GetRemainingAttempts(Corpse corpse, int maxAttempts = 3)
    {
        if (corpse == null)
        {
            return 0;
        }

        var key = corpse.GetUniqueLoadID();
        return _corpseExtractionAttempts.TryGetValue(key, out var used)
            ? System.Math.Max(0, maxAttempts - used)
            : maxAttempts;
    }

    public void RecordExtractionAttempt(Corpse corpse)
    {
        if (corpse == null)
        {
            return;
        }

        var key = corpse.GetUniqueLoadID();
        _corpseExtractionAttempts[key] = _corpseExtractionAttempts.TryGetValue(key, out var used) ? used + 1 : 1;
    }

    public void AddRecord(ShadowRecord record)
    {
        if (record == null || string.IsNullOrEmpty(record.ShadowId))
        {
            return;
        }

        _records.Add(record);
    }

    public void MarkArmyUnavailable()
    {
        foreach (var record in _records)
        {
            if (record.State != ShadowState.Unavailable)
            {
                record.State = ShadowState.Unavailable;
            }
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref _schemaVersion, "schemaVersion", CurrentSchemaVersion);
        Scribe_References.Look(ref _monarchPawn, "monarchPawn");
        Scribe_Values.Look(ref _monarchId, "monarchId", string.Empty);
        Scribe_Collections.Look(ref _records, "records", LookMode.Deep);
        Scribe_Collections.Look(ref _corpseExtractionAttempts, "corpseExtractionAttempts", LookMode.Value, LookMode.Value);

        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            _records ??= new List<ShadowRecord>();
            _corpseExtractionAttempts ??= new Dictionary<string, int>();
            ReconcileAfterLoad();
        }
    }

    private void ReconcileAfterLoad()
    {
        if (_monarchPawn != null && (_monarchPawn.Dead || _monarchPawn.Destroyed))
        {
            MarkArmyUnavailable();
        }
    }
}
