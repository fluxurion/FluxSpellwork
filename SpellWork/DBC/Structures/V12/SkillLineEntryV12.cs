using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V12
{
    public sealed class SkillLineEntryV12
    {
        public string DisplayName;
        public string AlternateVerb;
        public string Description;
        public string HordeDisplayName;
        public string OverrideSourceInfoDisplayName;
        [Index(false)]
        public uint ID;
        public sbyte CategoryID;
        public int SpellIconFileID;
        public sbyte CanLink;
        public uint ParentSkillLineID;
        public int ParentTierIndex;
        public int Flags;
        public int SpellBookSpellID;
        public int ExpansionNameSharedStringID;
        public int HordeExpansionNameSharedStringID;
    }
}
