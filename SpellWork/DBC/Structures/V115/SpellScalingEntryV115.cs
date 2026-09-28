using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class SpellScalingEntryV115 : IConvertsTo<SpellScalingEntry>
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        public int Class;
        public uint MinScalingLevel;
        public uint MaxScalingLevel;
        public short ScalesFromItemLevel;
        public int CastTimeMin;
        public int CastTimeMax;
        public int CastTimeMaxLevel;
        public float NerfFactor;
        public int NerfMaxLevel;

        public SpellScalingEntry ToCanonical()
        {
            var e = new SpellScalingEntry
            {
                ID = (uint)ID,
                SpellID = SpellID,
                MinScalingLevel = MinScalingLevel,
                MaxScalingLevel = MaxScalingLevel,
                ScalesFromItemLevel = ScalesFromItemLevel,
            };
            return e;
        }
    }
}
