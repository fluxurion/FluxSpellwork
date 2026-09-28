using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V12
{
    public class SpellLevelsEntryV12
    {
        [Index(true)]
        public uint ID;
        public short DifficultyID;
        public short MaxLevel;
        public byte MaxPassiveAuraLevel;
        public int BaseLevel;
        public int SpellLevel;
        public int SpellID;
    }
}
