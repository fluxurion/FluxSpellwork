using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class SpellXDescriptionVariablesEntryV115 : IConvertsTo<SpellXDescriptionVariablesEntry>
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
