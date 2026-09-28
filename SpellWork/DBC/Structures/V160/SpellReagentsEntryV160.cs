using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V160
{
    public sealed class SpellReagentsEntryV160 : IConvertsTo<SpellReagentsEntry>
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        [Cardinality(8)]
        public int[] Reagent = new int[8];
        [Cardinality(8)]
        public short[] ReagentCount = new short[8];
        [Cardinality(8)]
        public short[] ReagentReCraftCount = new short[8];
        [Cardinality(8)]
        public byte[] ReagentSource = new byte[8];

        public SpellReagentsEntry ToCanonical()
        {
            var e = new SpellReagentsEntry
            {
                ID = (uint)ID,
                SpellID = SpellID,
                Reagent = Reagent,
                ReagentCount = ReagentCount,
                ReagentRecraftCount = ReagentReCraftCount,
                ReagentSource = ReagentSource,
            };
            return e;
        }
    }
}
