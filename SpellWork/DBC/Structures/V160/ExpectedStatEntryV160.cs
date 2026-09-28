using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V160
{
    public sealed class ExpectedStatEntryV160 : IConvertsTo<ExpectedStatEntry>
    {
        [Index(true)]
        public int ID;
        public int ExpansionID;
        public float CreatureHealth;
        public float PlayerHealth;
        public float CreatureAutoAttackDps;
        public float CreatureArmor;
        public float PlayerMana;
        public float PlayerPrimaryStat;
        public float PlayerSecondaryStat;
        public float ArmorConstant;
        public float CreatureSpellDamage;
        public int ContentSetID;
        public int Lvl;

        public ExpectedStatEntry ToCanonical()
        {
            var e = new ExpectedStatEntry
            {
                ID = (uint)ID,
                ExpansionID = ExpansionID,
                CreatureHealth = CreatureHealth,
                PlayerHealth = PlayerHealth,
                CreatureAutoAttackDps = CreatureAutoAttackDps,
                CreatureArmor = CreatureArmor,
                PlayerMana = PlayerMana,
                PlayerPrimaryStat = PlayerPrimaryStat,
                PlayerSecondaryStat = PlayerSecondaryStat,
                ArmorConstant = ArmorConstant,
                CreatureSpellDamage = CreatureSpellDamage,
                Lvl = (uint)Lvl,
            };
            return e;
        }
    }
}
