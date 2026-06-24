using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V8
{
    public class SpellScalingEntryV8
    {
        [Index(true)]
        public uint ID;
        public int SpellID;
        public int Class;
        public uint MinScalingLevel;
        public uint MaxScalingLevel;
        public short ScalesFromItemLevel;
    }
}
