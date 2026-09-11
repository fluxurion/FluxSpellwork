using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V12
{
    public sealed class SpellRangeEntryV12
    {
        [Index(true)]
        public uint ID;
        public string DisplayName;
        public string DisplayNameShort;
        public int Flags;
        [Cardinality(2)]
        public float[] MinRange = new float[2];
        [Cardinality(2)]
        public float[] MaxRange = new float[2];
    }
}
