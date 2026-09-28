using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class SpellEntryV115 : IConvertsTo<SpellEntry>
    {
        [Index(true)]
        public int ID;
        public string NameSubtext_lang;
        public string Description_lang;
        public string AuraDescription_lang;

        public SpellEntry ToCanonical()
        {
            var e = new SpellEntry
            {
                ID = (uint)ID,
                NameSubtext = NameSubtext_lang,
                Description = Description_lang,
                AuraDescription = AuraDescription_lang,
            };
            return e;
        }
    }
}
