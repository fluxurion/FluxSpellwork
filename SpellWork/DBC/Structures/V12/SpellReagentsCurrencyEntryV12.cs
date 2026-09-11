using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V12
{
    public class SpellReagentsCurrencyEntryV12
    {
        [Index(true)]
        public uint ID;
        public int SpellID;
        public int CurrencyTypesID;
        public int CurrencyCount;
        public int OverrideRecraftCurrencyCount;
        public byte OrderSource;
    }
}
