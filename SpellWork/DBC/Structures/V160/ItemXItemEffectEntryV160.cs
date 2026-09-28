using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V160
{
    public sealed class ItemXItemEffectEntryV160 : IConvertsTo<ItemXItemEffectEntry>
    {
        [Index(true)]
        public int ID;
        public int ItemEffectID;
        public int ItemID;

        public ItemXItemEffectEntry ToCanonical()
        {
            var e = new ItemXItemEffectEntry
            {
                ID = (uint)ID,
                ItemEffectID = ItemEffectID,
                ItemID = ItemID,
            };
            return e;
        }
    }
}
