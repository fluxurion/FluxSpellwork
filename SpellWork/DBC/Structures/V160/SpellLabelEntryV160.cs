using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V160
{
    public sealed class SpellLabelEntryV160 : IConvertsTo<SpellLabelEntry>
    {
        [Index(true)]
        public int ID;
        public uint LabelID;
        public int SpellID;

        public SpellLabelEntry ToCanonical()
        {
            var e = new SpellLabelEntry
            {
                ID = (uint)ID,
                LabelID = LabelID,
                SpellID = SpellID,
            };
            return e;
        }
    }
}
