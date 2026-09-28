using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V160
{
    public sealed class ContentTuningEntryV160 : IConvertsTo<ContentTuningEntry>
    {
        [Index(false)]
        public int ID;
        public int Flags;
        public int ExpansionID;
        public int HPScalingCurveID;
        public int DMGScalingCurveID;
        public int HPPrimaryStatScalingCurveID;
        public int DMGPrimaryStatScalingCurveID;
        public int PrimaryStatScalingModPlayerDataElementCharacterID;
        public float PrimaryStatScalingModPlayerDataElementCharacterMultiplier;
        public int MinLevelSquish;
        public int MaxLevelSquish;
        public int MinLevelScalingOffset;
        public int MaxLevelScalingOffset;
        public int AllowedMinOffset;
        public int AllowedMaxOffset;
        public int LfgMinLevel;
        public int LfgMaxLevel;
        public int ILevel;
        public float XpMultQuest;

        public ContentTuningEntry ToCanonical()
        {
            var e = new ContentTuningEntry
            {
                ID = (uint)ID,
                Flags = Flags,
                ExpansionID = ExpansionID,
            };
            return e;
        }
    }
}
