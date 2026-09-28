using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class SpellDurationEntryV115 : IConvertsTo<SpellDurationEntry>
    {
        [Index(true)]
        public int ID;
        public int Duration;
        public uint DurationPerLevel;
        public int MaxDuration;

        public SpellDurationEntry ToCanonical()
        {
            var e = new SpellDurationEntry
            {
                ID = (uint)ID,
                Duration = Duration,
                MaxDuration = MaxDuration,
            };
            return e;
        }
    }
}
