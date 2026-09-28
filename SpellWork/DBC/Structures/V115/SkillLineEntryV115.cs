using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class SkillLineEntryV115 : IConvertsTo<SkillLineEntry>
    {
        public string DisplayName_lang;
        public string AlternateVerb_lang;
        public string Description_lang;
        public string HordeDisplayName_lang;
        public string NeutralDisplayName;
        [Index(false)]
        public int ID;
        public sbyte CategoryID;
        public int SpellIconFileID;
        public sbyte CanLink;
        public uint ParentSkillLineID;
        public int ParentTierIndex;
        public int Flags;
        public int SpellBookSpellID;

        public SkillLineEntry ToCanonical()
        {
            var e = new SkillLineEntry
            {
                DisplayName = DisplayName_lang,
                AlternateVerb = AlternateVerb_lang,
                Description = Description_lang,
                HordeDisplayName = HordeDisplayName_lang,
                OverrideSourceInfoDisplayName = NeutralDisplayName,
                ID = (uint)ID,
                CategoryID = CategoryID,
                SpellIconFileID = SpellIconFileID,
                CanLink = CanLink,
                ParentSkillLineID = ParentSkillLineID,
                ParentTierIndex = ParentTierIndex,
                Flags = (ushort)Flags,
                SpellBookSpellID = SpellBookSpellID,
            };
            return e;
        }
    }
}
