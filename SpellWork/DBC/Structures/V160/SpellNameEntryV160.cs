using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V160
{
    public sealed class SpellNameEntryV160 : IConvertsTo<SpellNameEntry>
    {
        [Index(true)]
        public int ID;
        public string Name_lang;

        public SpellNameEntry ToCanonical()
        {
            var e = new SpellNameEntry
            {
                ID = (uint)ID,
                Name = Name_lang,
            };
            return e;
        }
    }
}
