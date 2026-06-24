using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V7
{
    public class SpellEquippedItemsEntryV7
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        public uint EquippedItemInventoryTypeMask;
        public uint EquippedItemSubClassMask;
        public byte EquippedItemClass;
    }
}
