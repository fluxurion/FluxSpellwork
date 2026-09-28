using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V160
{
    public sealed class RandPropPointsEntryV160 : IConvertsTo<RandPropPointsEntry>
    {
        [Index(true)]
        public int ID;
        public float DamageReplaceStatF;
        public float DamageSecondaryF;
        public int DamageReplaceStat;
        public int DamageSecondary;
        [Cardinality(5)]
        public float[] EpicF = new float[5];
        [Cardinality(5)]
        public float[] SuperiorF = new float[5];
        [Cardinality(5)]
        public float[] GoodF = new float[5];
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
                DamageReplaceStatF = DamageReplaceStatF,
                DamageSecondaryF = DamageSecondaryF,
                DamageReplaceStat = DamageReplaceStat,
                DamageSecondary = DamageSecondary,
                EpicF = EpicF,
                SuperiorF = SuperiorF,
                GoodF = GoodF,
                Epic = Epic,
                Superior = Superior,
                Good = Good,
            };
            return e;
        }
    }
}
