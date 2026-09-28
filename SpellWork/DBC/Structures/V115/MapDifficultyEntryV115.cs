using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class MapDifficultyEntryV115 : IConvertsTo<MapDifficultyEntry>
    {
        public string Message_lang;
        [Index(false)]
        public int ID;
        public short DifficultyID;
        public int LockID;
        public byte ResetInterval;
        public int MaxPlayers;
        public byte ItemContext;
        public int ItemContextPickerID;
        public int Flags;
        public int ContentTuningID;
        public int WorldStateExpressionID;
        public int MapID;

        public MapDifficultyEntry ToCanonical()
        {
            var e = new MapDifficultyEntry
            {
                Message = Message_lang,
                ID = (uint)ID,
                DifficultyID = (int)DifficultyID,
                LockID = LockID,
                ResetInterval = (sbyte)ResetInterval,
                MaxPlayers = MaxPlayers,
                ItemContext = (int)ItemContext,
                ItemContextPickerID = ItemContextPickerID,
                Flags = Flags,
                ContentTuningID = ContentTuningID,
                MapID = MapID,
            };
            return e;
        }
    }
}
