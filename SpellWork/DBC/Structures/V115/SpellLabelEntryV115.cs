using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class SpellLabelEntryV115 : IConvertsTo<SpellLabelEntry>
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
