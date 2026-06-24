using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V7
{
    public class SpellScalingEntryV7
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        public ushort ScalesFromItemLevel;
        public int ScalingClass;
        public uint MinScalingLevel;
        public uint MaxScalingLevel;
    }
}
