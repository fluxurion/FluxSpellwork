using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V12
{
    public class SpellScalingEntryV12
    {
        [Index(true)]
        public uint ID;
        public int SpellID;
        public uint MinScalingLevel;
        public uint MaxScalingLevel;
    }
}
