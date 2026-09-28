using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V160
{
    public sealed class SpellAuraRestrictionsEntryV160 : IConvertsTo<SpellAuraRestrictionsEntry>
    {
        [Index(true)]
        public int ID;
        public short DifficultyID;
        public int CasterAuraState;
        public int TargetAuraState;
        public int ExcludeCasterAuraState;
        public int ExcludeTargetAuraState;
        public int CasterAuraSpell;
        public int TargetAuraSpell;
        public int ExcludeCasterAuraSpell;
        public int ExcludeTargetAuraSpell;
        public short CasterAuraType;
        public short TargetAuraType;
        public short ExcludeCasterAuraType;
        public short ExcludeTargetAuraType;
        public int SpellID;

        public SpellAuraRestrictionsEntry ToCanonical()
        {
            var e = new SpellAuraRestrictionsEntry
            {
                ID = (uint)ID,
                DifficultyID = (int)DifficultyID,
                CasterAuraState = CasterAuraState,
                TargetAuraState = TargetAuraState,
                ExcludeCasterAuraState = ExcludeCasterAuraState,
                ExcludeTargetAuraState = ExcludeTargetAuraState,
                CasterAuraSpell = CasterAuraSpell,
                TargetAuraSpell = TargetAuraSpell,
                ExcludeCasterAuraSpell = ExcludeCasterAuraSpell,
                ExcludeTargetAuraSpell = ExcludeTargetAuraSpell,
                CasterAuraType = (int)CasterAuraType,
                TargetAuraType = (int)TargetAuraType,
                ExcludeCasterAuraType = (int)ExcludeCasterAuraType,
                ExcludeTargetAuraType = (int)ExcludeTargetAuraType,
                SpellID = SpellID,
            };
            return e;
        }
    }
}
