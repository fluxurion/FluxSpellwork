using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V12
{
    public class SpellCategoryEntryV12
    {
        [Index(true)]
        public uint ID;
        public string Name;
        public int Flags;
        public int UsesPerWeek;
        public int MaxCharges;
        public int ChargeRecoveryTime;
        public int TypeMask;
    };
}
