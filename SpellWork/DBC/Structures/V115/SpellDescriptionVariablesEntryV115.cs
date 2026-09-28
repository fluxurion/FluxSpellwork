using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class SpellDescriptionVariablesEntryV115 : IConvertsTo<SpellDescriptionVariablesEntry>
    {
        [Index(true)]
        public int ID;
        public string Variables;

        public SpellDescriptionVariablesEntry ToCanonical()
        {
            var e = new SpellDescriptionVariablesEntry
            {
                ID = (uint)ID,
                Variables = Variables,
            };
            return e;
        }
    }
}
