using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V7
{
    public sealed class SpellXSpellVisualEntryV7
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        public uint SpellVisualID;
        public float Chance;
        public ushort CasterPlayerConditionID;
        public ushort CasterUnitConditionID;
        public ushort PlayerConditionID;
        public ushort UnitConditionID;
        public uint IconFileDataID;
        public uint ActiveIconFileDataID;
        public byte Flags;
        public byte DifficultyID;
        public byte Priority;
    }
}
