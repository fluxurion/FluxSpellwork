using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V160
{
    public sealed class SkillLineAbilityEntryV160 : IConvertsTo<SkillLineAbilityEntry>
    {
        public string AbilityVerb_lang;
        public string AbilityAllVerb_lang;
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
        public short SkillupSkillLineID;
        [Cardinality(2)]
        public int[] Field_5_5_4_67090_014 = new int[2];
        [Cardinality(2)]
        public int[] RaceMasks = new int[2];

        public SkillLineAbilityEntry ToCanonical()
        {
            var e = new SkillLineAbilityEntry
            {
                AbilityVerb = AbilityVerb_lang,
                AbilityAllVerb = AbilityAllVerb_lang,
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
                SkillupSkillLineID = SkillupSkillLineID,
                RaceMask = (long)((RaceMasks[0] & 0xFFFFFFFFL) | ((long)RaceMasks[1] << 32)),
            };
            return e;
        }
    }
}
