using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V8
{
    public class SpellCooldownsEntryV8
    {
        [Index(true)]
        public uint ID;
        public byte DifficultyID;
        public int CategoryRecoveryTime;
        public int RecoveryTime;
        public int StartRecoveryTime;
        public int SpellID;
    }
}
