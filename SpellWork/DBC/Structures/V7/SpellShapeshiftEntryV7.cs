using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V7
{
    public class SpellShapeshiftEntryV7
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        [Cardinality(2)]
        public uint[] ShapeshiftExclude = new uint[2];
        [Cardinality(2)]
        public uint[] ShapeshiftMask = new uint[2];
        public int StanceBarOrder;
    }
}
