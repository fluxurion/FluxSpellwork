using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V7
{
    public sealed class SpellEntryV7
    {
        [Index(true)]
        public int ID;
        public string Name;
        public string NameSubtext;
        public string Description;
        public string AuraDescription;
        public int MiscID;
        public int DescriptionVariablesID;
    }
}
