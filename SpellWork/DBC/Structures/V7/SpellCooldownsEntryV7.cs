using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V7
{
    public class SpellCooldownsEntryV7
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        public uint CategoryRecoveryTime;
        public uint RecoveryTime;
        public uint StartRecoveryTime;
        public byte DifficultyID;
    }
}
