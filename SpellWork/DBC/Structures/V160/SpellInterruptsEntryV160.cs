using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V160
{
    public sealed class SpellInterruptsEntryV160 : IConvertsTo<SpellInterruptsEntry>
    {
        [Index(true)]
        public int ID;
        public short DifficultyID;
        public int InterruptFlags;
        [Cardinality(2)]
        public int[] AuraInterruptFlags = new int[2];
        [Cardinality(2)]
        public int[] ChannelInterruptFlags = new int[2];
        public int SpellID;

        public SpellInterruptsEntry ToCanonical()
        {
            var e = new SpellInterruptsEntry
            {
                ID = (uint)ID,
                DifficultyID = (byte)DifficultyID,
                InterruptFlags = (short)InterruptFlags,
                AuraInterruptFlags = AuraInterruptFlags,
                ChannelInterruptFlags = ChannelInterruptFlags,
                SpellID = SpellID,
            };
            return e;
        }
    }
}
