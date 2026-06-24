using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V7
{
    public class SpellReagentsCurrencyEntryV7
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        public ushort CurrencyTypeID;
        public ushort CurrencyCount;
    }
}
