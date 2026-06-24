using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V7
{
    public sealed class SpellDescriptionVariablesEntryV7
    {
        [Index(true)]
        public int ID;
        public string Variables;
    }
}
