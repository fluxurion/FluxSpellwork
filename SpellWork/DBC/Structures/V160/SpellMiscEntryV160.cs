using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V160
{
    public sealed class SpellMiscEntryV160 : IConvertsTo<SpellMiscEntry>
    {
        [Index(true)]
        public int ID;
        [Cardinality(17)]
        public int[] Attributes = new int[17];
        public short DifficultyID;
        public ushort CastingTimeIndex;
        public ushort DurationIndex;
        public ushort PvPDurationIndex;
        public ushort RangeIndex;
        public byte SchoolMask;
        public float Speed;
        public float LaunchDelay;
        public float MinDuration;
        public int SpellIconFileDataID;
        public int ActiveIconFileDataID;
        public int ContentTuningID;
        public int ShowFutureSpellPlayerConditionID;
        public int SpellVisualScript;
        public int ActiveSpellVisualScript;
        public int SpellID;

        public SpellMiscEntry ToCanonical()
        {
            var e = new SpellMiscEntry
            {
                ID = (uint)ID,
                DifficultyID = (byte)DifficultyID,
                CastingTimeIndex = CastingTimeIndex,
                DurationIndex = DurationIndex,
                RangeIndex = RangeIndex,
                SchoolMask = SchoolMask,
                Speed = Speed,
                LaunchDelay = LaunchDelay,
                MinDuration = MinDuration,
                SpellIconFileDataID = SpellIconFileDataID,
                ActiveIconFileDataID = ActiveIconFileDataID,
                ContentTuningID = ContentTuningID,
                ShowFutureSpellPlayerConditionID = ShowFutureSpellPlayerConditionID,
                SpellVisualScript = SpellVisualScript,
                ActiveSpellVisualScript = ActiveSpellVisualScript,
                SpellID = SpellID,
            };
            System.Array.Copy(Attributes, e.Attributes, 15);
            return e;
        }
    }
}
