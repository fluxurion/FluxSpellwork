using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class OverrideSpellDataEntryV115 : IConvertsTo<OverrideSpellDataEntry>
    {
        [Index(true)]
        public int ID;
        [Cardinality(10)]
        public int[] Spells = new int[10];
        public int PlayerActionbarFileDataID;
        public int Flags;

        public OverrideSpellDataEntry ToCanonical()
        {
            var e = new OverrideSpellDataEntry
            {
                ID = (uint)ID,
                Spells = Spells,
                PlayerActionbarFileDataID = PlayerActionbarFileDataID,
                Flags = (byte)Flags,
            };
            return e;
        }
    }
}
