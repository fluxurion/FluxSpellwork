using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class SpellCategoriesEntryV115 : IConvertsTo<SpellCategoriesEntry>
    {
        [Index(true)]
        public int ID;
        public short DifficultyID;
        public short Category;
        public sbyte DefenseType;
        public sbyte DispelType;
        public sbyte Mechanic;
        public int PreventionType;
        public short StartRecoveryCategory;
        public short ChargeCategory;
        public int SpellID;

        public SpellCategoriesEntry ToCanonical()
        {
            var e = new SpellCategoriesEntry
            {
                ID = (uint)ID,
                DifficultyID = (byte)DifficultyID,
                Category = Category,
                DefenseType = DefenseType,
                DispelType = DispelType,
                Mechanic = Mechanic,
                PreventionType = (sbyte)PreventionType,
                StartRecoveryCategory = StartRecoveryCategory,
                ChargeCategory = ChargeCategory,
                SpellID = SpellID,
            };
            return e;
        }
    }
}
