using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V160
{
    public sealed class SpellScalingEntryV160 : IConvertsTo<SpellScalingEntry>
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        public uint MinScalingLevel;
        public uint MaxScalingLevel;

        public SpellScalingEntry ToCanonical()
        {
            var e = new SpellScalingEntry
            {
                ID = (uint)ID,
                SpellID = SpellID,
                MinScalingLevel = MinScalingLevel,
                MaxScalingLevel = MaxScalingLevel,
            };
            return e;
        }
    }
}
