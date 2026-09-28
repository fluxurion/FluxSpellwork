using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V160
{
    public sealed class SpellLevelsEntryV160 : IConvertsTo<SpellLevelsEntry>
    {
        [Index(true)]
        public int ID;
        public short DifficultyID;
        public short MaxLevel;
        public byte MaxPassiveAuraLevel;
        public int BaseLevel;
        public int SpellLevel;
        public int SpellID;

        public SpellLevelsEntry ToCanonical()
        {
            var e = new SpellLevelsEntry
            {
                ID = (uint)ID,
                DifficultyID = (byte)DifficultyID,
                MaxLevel = MaxLevel,
                MaxPassiveAuraLevel = MaxPassiveAuraLevel,
                BaseLevel = BaseLevel,
                SpellLevel = SpellLevel,
                SpellID = SpellID,
            };
            return e;
        }
    }
}
