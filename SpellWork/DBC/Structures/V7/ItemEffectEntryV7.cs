using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V7
{
    public sealed class ItemEffectEntryV7
    {
        [Index(true)]
        public int ID;
        public uint ItemID;
        public uint SpellID;
        public int Cooldown;
        public int CategoryCooldown;
        public short Charges;
        public ushort Category;
        public ushort ChrSpecializationID;
        public byte OrderIndex;
        public byte Trigger;
    }
}
