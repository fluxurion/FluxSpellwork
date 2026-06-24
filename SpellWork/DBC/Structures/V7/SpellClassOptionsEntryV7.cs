using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V7
{
    public class SpellClassOptionsEntryV7
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        [Cardinality(4)]
        public uint[] SpellFamilyFlags = new uint[4];
        public byte SpellClassSet;
        public uint ModalNextSpell;
    }
}
