using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V160
{
    public sealed class SpellDurationEntryV160 : IConvertsTo<SpellDurationEntry>
    {
        [Index(true)]
        public int ID;
        public int Duration;
        public int MaxDuration;
        public int DurationPerResource;

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
