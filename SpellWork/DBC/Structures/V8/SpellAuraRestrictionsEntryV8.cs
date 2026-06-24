using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V8
{
    public class SpellAuraRestrictionsEntryV8
    {
        [Index(true)]
        public uint ID;
        public byte DifficultyID;
        public byte CasterAuraState;
        public byte TargetAuraState;
        public byte ExcludeCasterAuraState;
        public byte ExcludeTargetAuraState;
        public int CasterAuraSpell;
        public int TargetAuraSpell;
        public int ExcludeCasterAuraSpell;
        public int ExcludeTargetAuraSpell;
        public int SpellID;
    }
}
