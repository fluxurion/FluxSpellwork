using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V8
{
    public sealed class ItemEffectEntryV8
    {
        [Index(true)]
        public uint ID;
        public byte LegacySlotIndex;
        public sbyte TriggerType;
        public short Charges;
        public int CoolDownMSec;
        public int CategoryCoolDownMSec;
        public ushort SpellCategoryID;
        public int SpellID;
        public ushort ChrSpecializationID;
        public int ParentItemID;
    }
}
