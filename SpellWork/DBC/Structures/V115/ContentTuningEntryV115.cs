using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class ContentTuningEntryV115 : IConvertsTo<ContentTuningEntry>
    {
        [Index(false)]
        public int ID;
        public int MinLevelSquish;
        public int MaxLevelSquish;
        public int Flags;
        public int Field_1_15_8_63829_004;
        public int Field_1_15_8_63829_005;
        public int Field_1_15_8_63829_006;
        public int Field_1_15_8_63829_007;
        public int Field_1_15_8_63829_008;
        public int Field_1_15_8_63829_009;
        public int Field_1_15_8_63829_010;
        public float XpMultQuest;
        public int AllowedMinOffset;
        public int AllowedMaxOffset;

        public ContentTuningEntry ToCanonical()
        {
            var e = new ContentTuningEntry
            {
                ID = (uint)ID,
                Flags = Flags,
            };
            return e;
        }
    }
}
