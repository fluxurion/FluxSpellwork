using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V12
{
    public class SpellEquippedItemsEntryV12
    {
        [Index(true)]
        public uint ID;
        public int SpellID;
        public int EquippedItemClass;
        public int EquippedItemInvTypes;
        public int EquippedItemSubclass;
    }
}
