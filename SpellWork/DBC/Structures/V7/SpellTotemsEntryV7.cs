using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V7
{
    public class SpellTotemsEntryV7
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        [Cardinality(2)]
        public uint[] Totem = new uint[2];
        [Cardinality(2)]
        public ushort[] RequiredTotemCategoryID = new ushort[2];
    }
}
