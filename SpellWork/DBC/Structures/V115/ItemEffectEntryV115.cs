using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class ItemEffectEntryV115 : IConvertsTo<ItemEffectEntry>
    {
        [Index(true)]
        public int ID;
        public byte LegacySlotIndex;
        public byte TriggerType;
        public short Charges;
        public int CoolDownMSec;
        public int CategoryCoolDownMSec;
        public ushort SpellCategoryID;
        public int SpellID;
        public ushort ChrSpecializationID;
        public int PlayerConditionID;
        public int ParentItemID;

        public ItemEffectEntry ToCanonical()
        {
            var e = new ItemEffectEntry
            {
                ID = (uint)ID,
                LegacySlotIndex = LegacySlotIndex,
                TriggerType = (sbyte)TriggerType,
                Charges = Charges,
                CoolDownMSec = CoolDownMSec,
                CategoryCoolDownMSec = CategoryCoolDownMSec,
                SpellCategoryID = SpellCategoryID,
                SpellID = SpellID,
                ChrSpecializationID = ChrSpecializationID,
            };
            return e;
        }
    }
}
