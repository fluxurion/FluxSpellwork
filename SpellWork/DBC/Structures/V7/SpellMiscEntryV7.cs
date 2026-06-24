using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V7
{
    public class SpellMiscEntryV7
    {
        [Index(true)]
        public int ID;
        [Cardinality(14)]
        public uint[] Attributes = new uint[14];
        public float Speed;
        public float MultistrikeSpeedMod;
        public ushort CastingTimeIndex;
        public ushort DurationIndex;
        public ushort RangeIndex;
        public byte SchoolMask;
        public uint IconFileDataID;
        public uint ActiveIconFileDataID;
    }
}
