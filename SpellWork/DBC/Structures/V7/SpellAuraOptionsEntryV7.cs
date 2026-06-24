using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V7
{
    public class SpellAuraOptionsEntryV7
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        public uint ProcCharges;
        public uint ProcTypeMask;
        public uint ProcCategoryRecovery;
        public ushort CumulativeAura;
        public byte DifficultyID;
        public byte ProcChance;
        public byte SpellProcsPerMinuteID;
    }
}
