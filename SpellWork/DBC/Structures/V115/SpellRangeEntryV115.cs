using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class SpellRangeEntryV115 : IConvertsTo<SpellRangeEntry>
    {
        [Index(true)]
        public int ID;
        public string DisplayName_lang;
        public string DisplayNameShort_lang;
        public int Flags;
        [Cardinality(2)]
        public float[] RangeMin = new float[2];
        [Cardinality(2)]
        public float[] RangeMax = new float[2];

        public SpellRangeEntry ToCanonical()
        {
            var e = new SpellRangeEntry
            {
                ID = (uint)ID,
                DisplayName = DisplayName_lang,
                DisplayNameShort = DisplayNameShort_lang,
                Flags = (byte)Flags,
                MinRange = RangeMin,
                MaxRange = RangeMax,
            };
            return e;
        }
    }
}
