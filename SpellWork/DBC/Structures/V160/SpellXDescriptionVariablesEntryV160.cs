using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V160
{
    public sealed class SpellXDescriptionVariablesEntryV160 : IConvertsTo<SpellXDescriptionVariablesEntry>
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        public int SpellDescriptionVariablesID;

        public SpellXDescriptionVariablesEntry ToCanonical()
        {
            var e = new SpellXDescriptionVariablesEntry
            {
                ID = (uint)ID,
                SpellID = SpellID,
                SpellDescriptionVariablesID = SpellDescriptionVariablesID,
            };
            return e;
        }
    }
}
