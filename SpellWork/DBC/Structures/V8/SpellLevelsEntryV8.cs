using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V8
{
    public class SpellLevelsEntryV8
    {
        [Index(true)]
        public uint ID;
        public byte DifficultyID;
        public short BaseLevel;
        public short MaxLevel;
        public short SpellLevel;
        public byte MaxPassiveAuraLevel;
        public int SpellID;
    }
}
