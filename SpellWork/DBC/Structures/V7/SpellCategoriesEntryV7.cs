using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V7
{
    public class SpellCategoriesEntryV7
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        public ushort Category;
        public ushort StartRecoveryCategory;
        public ushort ChargeCategory;
        public byte DifficultyID;
        public byte DefenseType;
        public byte DispelType;
        public byte Mechanic;
        public byte PreventionType;
    }
}
