using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V160
{
    public sealed class DifficultyEntryV160 : IConvertsTo<DifficultyEntry>
    {
        [Index(true)]
        public int ID;
        public string Name_lang;
        public byte InstanceType;
        public byte OrderIndex;
        public sbyte OldEnumValue;
        public short FallbackDifficultyID;
        public byte MinPlayers;
        public byte MaxPlayers;
        public int Flags;
        public byte ItemContext;
        public short ToggleDifficultyID;
        public uint GroupSizeHealthCurveID;
        public uint GroupSizeDmgCurveID;
        public uint GroupSizeSpellPointsCurveID;
        public int Field_1_15_4_56400_013;

        public DifficultyEntry ToCanonical()
        {
            var e = new DifficultyEntry
            {
                ID = (uint)ID,
                Name = Name_lang,
                InstanceType = InstanceType,
                OrderIndex = OrderIndex,
                OldEnumValue = OldEnumValue,
                FallbackDifficultyID = (byte)FallbackDifficultyID,
                MinPlayers = MinPlayers,
                MaxPlayers = MaxPlayers,
                Flags = (ushort)Flags,
                ItemContext = ItemContext,
                ToggleDifficultyID = (byte)ToggleDifficultyID,
                GroupSizeHealthCurveID = (ushort)GroupSizeHealthCurveID,
                GroupSizeDmgCurveID = (ushort)GroupSizeDmgCurveID,
                GroupSizeSpellPointsCurveID = (ushort)GroupSizeSpellPointsCurveID,
            };
            return e;
        }
    }
}
