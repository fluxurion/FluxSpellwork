using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class RandPropPointsEntryV115 : IConvertsTo<RandPropPointsEntry>
    {
        [Index(true)]
        public int ID;
        public int DamageReplaceStat;
        [Cardinality(5)]
        public uint[] Epic = new uint[5];
        [Cardinality(5)]
        public uint[] Superior = new uint[5];
        [Cardinality(5)]
        public uint[] Good = new uint[5];

        public RandPropPointsEntry ToCanonical()
        {
            var e = new RandPropPointsEntry
            {
                ID = (uint)ID,
                DamageReplaceStat = DamageReplaceStat,
                Epic = Epic,
                Superior = Superior,
                Good = Good,
            };
            return e;
        }
    }
}
