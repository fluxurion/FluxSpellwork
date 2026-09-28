using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V160
{
    public sealed class SpellCategoryEntryV160 : IConvertsTo<SpellCategoryEntry>
    {
        [Index(true)]
        public int ID;
        public string Name_lang;
        public int Flags;
        public int UsesPerWeek;
        public int MaxCharges;
        public int ChargeRecoveryTime;
        public int TypeMask;

        public SpellCategoryEntry ToCanonical()
        {
            var e = new SpellCategoryEntry
            {
                ID = (uint)ID,
                Name = Name_lang,
                Flags = (sbyte)Flags,
                UsesPerWeek = (byte)UsesPerWeek,
                MaxCharges = (sbyte)MaxCharges,
                ChargeRecoveryTime = ChargeRecoveryTime,
                TypeMask = TypeMask,
            };
            return e;
        }
    }
}
