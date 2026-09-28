using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class SpellTotemsEntryV115 : IConvertsTo<SpellTotemsEntry>
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        [Cardinality(2)]
        public ushort[] RequiredTotemCategoryID = new ushort[2];
        [Cardinality(2)]
        public int[] Totem = new int[2];

        public SpellTotemsEntry ToCanonical()
        {
            var e = new SpellTotemsEntry
            {
                ID = (uint)ID,
                SpellID = SpellID,
                RequiredTotemCategoryID = RequiredTotemCategoryID,
                Totem = Totem,
            };
            return e;
        }
    }
}
