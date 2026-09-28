using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class SpellCastTimesEntryV115 : IConvertsTo<SpellCastTimesEntry>
    {
        [Index(true)]
        public int ID;
        public int Base;
        public short PerLevel;
        public int Minimum;

        public SpellCastTimesEntry ToCanonical()
        {
            var e = new SpellCastTimesEntry
            {
                ID = (uint)ID,
                Base = Base,
                Minimum = Minimum,
            };
            return e;
        }
    }
}
