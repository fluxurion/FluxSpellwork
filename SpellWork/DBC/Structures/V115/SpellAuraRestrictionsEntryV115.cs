using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class SpellAuraRestrictionsEntryV115 : IConvertsTo<SpellAuraRestrictionsEntry>
    {
        [Index(true)]
        public int ID;
        public short DifficultyID;
        public byte CasterAuraState;
        public byte TargetAuraState;
        public byte ExcludeCasterAuraState;
        public byte ExcludeTargetAuraState;
        public int CasterAuraSpell;
        public int TargetAuraSpell;
        public int ExcludeCasterAuraSpell;
        public int ExcludeTargetAuraSpell;
        public int SpellID;

        public SpellAuraRestrictionsEntry ToCanonical()
        {
            var e = new SpellAuraRestrictionsEntry
            {
                ID = (uint)ID,
                DifficultyID = (int)DifficultyID,
                CasterAuraState = (int)CasterAuraState,
                TargetAuraState = (int)TargetAuraState,
                ExcludeCasterAuraState = (int)ExcludeCasterAuraState,
                ExcludeTargetAuraState = (int)ExcludeTargetAuraState,
                CasterAuraSpell = CasterAuraSpell,
                TargetAuraSpell = TargetAuraSpell,
                ExcludeCasterAuraSpell = ExcludeCasterAuraSpell,
                ExcludeTargetAuraSpell = ExcludeTargetAuraSpell,
                SpellID = SpellID,
            };
            return e;
        }
    }
}
