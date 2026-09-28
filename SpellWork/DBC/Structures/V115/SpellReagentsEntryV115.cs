using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class SpellReagentsEntryV115 : IConvertsTo<SpellReagentsEntry>
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        [Cardinality(8)]
        public int[] Reagent = new int[8];
        [Cardinality(8)]
        public short[] ReagentCount = new short[8];

        public SpellReagentsEntry ToCanonical()
        {
            var e = new SpellReagentsEntry
            {
                ID = (uint)ID,
                SpellID = SpellID,
                Reagent = Reagent,
                ReagentCount = ReagentCount,
            };
            return e;
        }
    }
}
