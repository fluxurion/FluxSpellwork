using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V160
{
    public sealed class SpellCooldownsEntryV160 : IConvertsTo<SpellCooldownsEntry>
    {
        [Index(true)]
        public int ID;
        public short DifficultyID;
        public int CategoryRecoveryTime;
        public int RecoveryTime;
        public int StartRecoveryTime;
        public int AuraSpellID;
        public int SpellID;

        public SpellCooldownsEntry ToCanonical()
        {
            var e = new SpellCooldownsEntry
            {
                ID = (uint)ID,
                DifficultyID = (byte)DifficultyID,
                CategoryRecoveryTime = CategoryRecoveryTime,
                RecoveryTime = RecoveryTime,
                StartRecoveryTime = StartRecoveryTime,
                AuraSpellID = AuraSpellID,
                SpellID = SpellID,
            };
            return e;
        }
    }
}
