using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V12
{
    public sealed class OverrideSpellDataEntryV12
    {
        [Index(true)]
        public uint ID;
        [Cardinality(10)]
        public int[] Spells = new int[10];
        public int PlayerActionbarFileDataID;
        public int Flags;
    }
}
