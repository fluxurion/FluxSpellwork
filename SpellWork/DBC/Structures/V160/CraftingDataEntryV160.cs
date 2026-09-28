using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V160
{
    public sealed class CraftingDataEntryV160 : IConvertsTo<CraftingDataEntry>
    {
        [Index(false)]
        public int ID;
        public int Type;
        public int CraftingDifficultyID;
        public int CraftedItemID;
        public int ItemBonusTreeID;
        public int CraftingDifficulty;
        public float Field_10_0_0_44649_005;
        public float CraftSkillBonusPercent;
        public float ReCraftSkillBonusPercent;
        public float InspirationSkillBonusPercent;
        public float Field_10_0_0_44649_009;
        public float Field_10_0_0_45141_011;
        public int FirstCraftFlagQuestID;
        public int FirstCraftTreasureID;
        public int FirstCraftPlayerDataFlagCharacterID;
        public int CraftedTreasureID;

        public CraftingDataEntry ToCanonical()
        {
            var e = new CraftingDataEntry
            {
                ID = (uint)ID,
                Type = Type,
                CraftingDifficultyID = CraftingDifficultyID,
                CraftedItemID = CraftedItemID,
                ItemBonusTreeID = ItemBonusTreeID,
                CraftingDifficulty = CraftingDifficulty,
                Field_10_0_0_44649_005 = Field_10_0_0_44649_005,
                CraftSkillBonusPercent = CraftSkillBonusPercent,
                ReCraftSkillBonusPercent = ReCraftSkillBonusPercent,
                InspirationSkillBonusPercent = InspirationSkillBonusPercent,
                Field_10_0_0_44649_009 = Field_10_0_0_44649_009,
                Field_10_0_0_45141_011 = Field_10_0_0_45141_011,
                FirstCraftFlagQuestID = FirstCraftFlagQuestID,
                FirstCraftTreasureID = FirstCraftTreasureID,
                FirstCraftPlayerDataFlagCharacterID = FirstCraftPlayerDataFlagCharacterID,
                CraftedTreasureID = CraftedTreasureID,
            };
            return e;
        }
    }
}
