using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V160
{
    public sealed class SpellXSpellVisualEntryV160 : IConvertsTo<SpellXSpellVisualEntry>
    {
        [Index(false)]
        public int ID;
        public short DifficultyID;
        public uint SpellVisualID;
        public float Probability;
        public int Flags2;
        public int Priority;
        public int SpellIconFileID;
        public int ActiveIconFileID;
        public ushort ViewerUnitConditionID;
        public uint ViewerPlayerConditionID;
        public ushort CasterUnitConditionID;
        public uint CasterPlayerConditionID;
        public int SpellID;

        public SpellXSpellVisualEntry ToCanonical()
        {
            var e = new SpellXSpellVisualEntry
            {
                ID = ID,
                DifficultyID = (byte)DifficultyID,
                SpellVisualID = SpellVisualID,
                Probability = Probability,
                Flags = Flags2,
                Priority = Priority,
                SpellIconFileID = SpellIconFileID,
                ActiveIconFileID = ActiveIconFileID,
                ViewerUnitConditionID = ViewerUnitConditionID,
                ViewerPlayerConditionID = ViewerPlayerConditionID,
                CasterUnitConditionID = CasterUnitConditionID,
                CasterPlayerConditionID = CasterPlayerConditionID,
                SpellID = SpellID,
            };
            return e;
        }
    }
}
