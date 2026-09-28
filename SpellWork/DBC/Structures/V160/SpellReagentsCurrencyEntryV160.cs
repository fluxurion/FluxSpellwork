using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V160
{
    public sealed class SpellReagentsCurrencyEntryV160 : IConvertsTo<SpellReagentsCurrencyEntry>
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        public int CurrencyTypesID;
        public int CurrencyCount;
        public int OverrideRecraftCurrencyCount;
        public byte OrderSource;

        public SpellReagentsCurrencyEntry ToCanonical()
        {
            var e = new SpellReagentsCurrencyEntry
            {
                ID = (uint)ID,
                SpellID = SpellID,
                CurrencyTypesID = (ushort)CurrencyTypesID,
                CurrencyCount = (ushort)CurrencyCount,
            };
            return e;
        }
    }
}
