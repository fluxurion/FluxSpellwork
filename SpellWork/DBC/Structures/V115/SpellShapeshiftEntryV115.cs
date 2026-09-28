using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class SpellShapeshiftEntryV115 : IConvertsTo<SpellShapeshiftEntry>
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        public sbyte StanceBarOrder;
        [Cardinality(2)]
        public int[] ShapeshiftExclude = new int[2];
        [Cardinality(2)]
        public int[] ShapeshiftMask = new int[2];

        public SpellShapeshiftEntry ToCanonical()
        {
            var e = new SpellShapeshiftEntry
            {
                ID = (uint)ID,
                SpellID = SpellID,
                StanceBarOrder = StanceBarOrder,
                ShapeshiftExclude = ShapeshiftExclude,
                ShapeshiftMask = ShapeshiftMask,
            };
            return e;
        }
    }
}
