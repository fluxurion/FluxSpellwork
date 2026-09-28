using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class SpellEquippedItemsEntryV115 : IConvertsTo<SpellEquippedItemsEntry>
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        public int EquippedItemClass;
        public int EquippedItemInvTypes;
        public int EquippedItemSubclass;

        public SpellEquippedItemsEntry ToCanonical()
        {
            var e = new SpellEquippedItemsEntry
            {
                ID = (uint)ID,
                SpellID = SpellID,
                EquippedItemClass = (sbyte)EquippedItemClass,
                EquippedItemInvTypes = EquippedItemInvTypes,
                EquippedItemSubclass = EquippedItemSubclass,
            };
            return e;
        }
    }
}
