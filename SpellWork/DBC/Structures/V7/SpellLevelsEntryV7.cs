using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V7
{
    public class SpellLevelsEntryV7
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        public ushort BaseLevel;
        public ushort MaxLevel;
        public ushort SpellLevel;
        public byte DifficultyID;
        public byte MaxUsableLevel;
    }
}
