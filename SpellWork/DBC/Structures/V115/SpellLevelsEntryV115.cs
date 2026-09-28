using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class SpellLevelsEntryV115 : IConvertsTo<SpellLevelsEntry>
    {
        [Index(true)]
        public int ID;
        public short DifficultyID;
        public short BaseLevel;
        public short MaxLevel;
        public short SpellLevel;
        public byte MaxPassiveAuraLevel;
        public int SpellID;

        public SpellLevelsEntry ToCanonical()
        {
            var e = new SpellLevelsEntry
            {
                ID = (uint)ID,
                DifficultyID = (byte)DifficultyID,
                BaseLevel = (int)BaseLevel,
                MaxLevel = MaxLevel,
                SpellLevel = (int)SpellLevel,
                MaxPassiveAuraLevel = MaxPassiveAuraLevel,
                SpellID = SpellID,
            };
            return e;
        }
    }
}
