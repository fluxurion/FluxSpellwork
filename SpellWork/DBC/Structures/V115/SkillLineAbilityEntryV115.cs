using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class SkillLineAbilityEntryV115 : IConvertsTo<SkillLineAbilityEntry>
    {
        [Index(false)]
        public int ID;
        public short SkillLine;
        public int Spell;
        public short MinSkillLineRank;
        public int ClassMask;
        public int SupercedesSpell;
        public int AcquireMethod;
        public short TrivialSkillLineRankHigh;
        public short TrivialSkillLineRankLow;
        public int Flags;
        public sbyte NumSkillUps;
        public short UniqueBit;
        public short TradeSkillCategoryID;
        public short Field_5_5_4_67090_013;
        [Cardinality(2)]
        public int[] Field_5_5_4_67090_014 = new int[2];
        [Cardinality(2)]
        public int[] RaceMask = new int[2];

        public SkillLineAbilityEntry ToCanonical()
        {
            var e = new SkillLineAbilityEntry
            {
                ID = (uint)ID,
                SkillLine = SkillLine,
                Spell = Spell,
                MinSkillLineRank = MinSkillLineRank,
                ClassMask = ClassMask,
                SupercedesSpell = SupercedesSpell,
                AcquireMethod = (sbyte)AcquireMethod,
                TrivialSkillLineRankHigh = TrivialSkillLineRankHigh,
                TrivialSkillLineRankLow = TrivialSkillLineRankLow,
                Flags = Flags,
                NumSkillUps = NumSkillUps,
                UniqueBit = UniqueBit,
                TradeSkillCategoryID = TradeSkillCategoryID,
                SkillupSkillLineID = Field_5_5_4_67090_013,
                RaceMask = (long)((RaceMask[0] & 0xFFFFFFFFL) | ((long)RaceMask[1] << 32)),
            };
            return e;
        }
    }
}
