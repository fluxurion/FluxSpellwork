using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V12
{
    public class DifficultyEntryV12
    {
        [Index(true)]
        public uint ID;
        public string Name;
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
        public int Unknown1105;
    }
}
