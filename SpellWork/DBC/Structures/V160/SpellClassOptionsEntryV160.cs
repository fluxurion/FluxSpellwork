using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V160
{
    public sealed class SpellClassOptionsEntryV160 : IConvertsTo<SpellClassOptionsEntry>
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        public uint ModalNextSpell;
        public int SpellClassSet;
        [Cardinality(4)]
        public int[] SpellClassMask = new int[4];

        public SpellClassOptionsEntry ToCanonical()
        {
            var e = new SpellClassOptionsEntry
            {
                ID = (uint)ID,
                SpellID = SpellID,
                ModalNextSpell = ModalNextSpell,
                SpellClassSet = (byte)SpellClassSet,
                SpellClassMask = SpellClassMask,
            };
            return e;
        }
    }
}
