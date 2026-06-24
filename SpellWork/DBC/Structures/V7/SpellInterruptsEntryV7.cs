using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V7
{
    public class SpellInterruptsEntryV7
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        [Cardinality(2)]
        public uint[] AuraInterruptFlags = new uint[2];
        [Cardinality(2)]
        public uint[] ChannelInterruptFlags = new uint[2];
        public ushort InterruptFlags;
        public byte DifficultyID;
    }
}
