using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V12
{
    public sealed class ItemEffectEntryV12
    {
        [Index(true)]
        public uint ID;
        public byte LegacySlotIndex;
        public byte TriggerType;
        public short Charges;
        public int CoolDownMSec;
        public int CategoryCoolDownMSec;
        public ushort SpellCategoryID;
        public int SpellID;
        public ushort ChrSpecializationID;
        public int PlayerConditionID;

        // Helper
        public ItemSparseEntryV12 Item { get; set; }
        public int ItemID { get; set; }
    }
}
