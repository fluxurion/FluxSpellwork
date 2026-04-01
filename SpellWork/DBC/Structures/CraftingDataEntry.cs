using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures
{
    public sealed class CraftingDataEntry
    {
        [Index(true)]
        public uint ID;
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
    }
}
