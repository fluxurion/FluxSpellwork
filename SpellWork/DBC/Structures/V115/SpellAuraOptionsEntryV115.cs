using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class SpellAuraOptionsEntryV115 : IConvertsTo<SpellAuraOptionsEntry>
    {
        [Index(true)]
        public int ID;
        public short DifficultyID;
        public uint CumulativeAura;
        public int ProcCategoryRecovery;
        public byte ProcChance;
        public int ProcCharges;
        public ushort SpellProcsPerMinuteID;
        [Cardinality(2)]
        public int[] ProcTypeMask = new int[2];
        public int SpellID;

        public SpellAuraOptionsEntry ToCanonical()
        {
            var e = new SpellAuraOptionsEntry
            {
                ID = (uint)ID,
                DifficultyID = (byte)DifficultyID,
                CumulativeAura = (ushort)CumulativeAura,
                ProcCategoryRecovery = ProcCategoryRecovery,
                ProcChance = ProcChance,
                ProcCharges = ProcCharges,
                SpellProcsPerMinuteID = SpellProcsPerMinuteID,
                ProcTypeMask = ProcTypeMask,
                SpellID = SpellID,
            };
            return e;
        }
    }
}
