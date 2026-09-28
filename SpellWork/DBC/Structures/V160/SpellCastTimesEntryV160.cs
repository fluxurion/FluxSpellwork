using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V160
{
    public sealed class SpellCastTimesEntryV160 : IConvertsTo<SpellCastTimesEntry>
    {
        [Index(true)]
        public int ID;
        public int Base;
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
