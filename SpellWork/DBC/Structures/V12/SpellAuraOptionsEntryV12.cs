using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V12
{
    public class SpellAuraOptionsEntryV12
    {
        [Index(true)]
        public uint ID;
        public short DifficultyID;
        public ushort CumulativeAura;
        public int ProcCategoryRecovery;
        public byte ProcChance;
        public int ProcCharges;
        public ushort SpellProcsPerMinuteID;
        [Cardinality(2)]
        public int[] ProcTypeMask = new int[2];
        public int SpellID;
    }
}
