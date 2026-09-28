using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V12
{
    public sealed class SpellDurationEntryV12
    {
        [Index(true)]
        public uint ID;
        public int Duration;
        public int MaxDuration;
        public int DurationPerResource;
    }
}
