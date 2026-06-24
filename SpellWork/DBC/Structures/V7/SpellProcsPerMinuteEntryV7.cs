using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V7
{
    public class SpellProcsPerMinuteEntryV7
    {
        [Index(true)]
        public int ID;
        public float BaseProcRate;
        public byte Flags;
    }
}
