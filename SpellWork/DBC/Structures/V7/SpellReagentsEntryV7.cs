using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V7
{
    public class SpellReagentsEntryV7
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        [Cardinality(8)]
        public uint[] Reagent = new uint[8];
        [Cardinality(8)]
        public ushort[] ReagentCount = new ushort[8];
    }
}
