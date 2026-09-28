using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V160
{
    public sealed class ContentTuningXExpectedEntryV160 : IConvertsTo<ContentTuningXExpectedEntry>
    {
        [Index(true)]
        public int ID;
        public int ExpectedStatModID;
        public int MinMythicPlusSeasonID;
        public int MaxMythicPlusSeasonID;
        public int ContentTuningID;

        public ContentTuningXExpectedEntry ToCanonical()
        {
            var e = new ContentTuningXExpectedEntry
            {
                ID = (uint)ID,
                ExpectedStatModID = ExpectedStatModID,
                MinMythicPlusSeasonID = MinMythicPlusSeasonID,
                MaxMythicPlusSeasonID = MaxMythicPlusSeasonID,
                ContentTuningID = (uint)ContentTuningID,
            };
            return e;
        }
    }
}
