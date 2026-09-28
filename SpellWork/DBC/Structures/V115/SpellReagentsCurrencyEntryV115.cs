using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class SpellReagentsCurrencyEntryV115 : IConvertsTo<SpellReagentsCurrencyEntry>
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        public ushort CurrencyTypesID;
        public ushort CurrencyCount;

        public SpellReagentsCurrencyEntry ToCanonical()
        {
            var e = new SpellReagentsCurrencyEntry
            {
                ID = (uint)ID,
                SpellID = SpellID,
                CurrencyTypesID = CurrencyTypesID,
                CurrencyCount = CurrencyCount,
            };
            return e;
        }
    }
}
