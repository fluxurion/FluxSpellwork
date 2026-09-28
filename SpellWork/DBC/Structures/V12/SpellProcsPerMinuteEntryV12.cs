using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V12
{
    public sealed class SpellProcsPerMinuteEntryV12
    {
        [Index(true)]
        public uint ID;
        public float BaseProcRate;
        public int Flags;
    }
}
