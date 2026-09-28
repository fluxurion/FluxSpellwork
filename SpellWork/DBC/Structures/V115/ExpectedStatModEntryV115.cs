using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class ExpectedStatModEntryV115 : IConvertsTo<ExpectedStatModEntry>
    {
        [Index(true)]
        public int ID;
        public float CreatureHealthMod;
        public float PlayerHealthMod;
        public float CreatureAutoAttackDPSMod;
        public float CreatureArmorMod;
        public float PlayerManaMod;
        public float PlayerPrimaryStatMod;
        public float PlayerSecondaryStatMod;
        public float ArmorConstantMod;
        public float CreatureSpellDamageMod;

        public ExpectedStatModEntry ToCanonical()
        {
            var e = new ExpectedStatModEntry
            {
                ID = (uint)ID,
                CreatureHealthMod = CreatureHealthMod,
                PlayerHealthMod = PlayerHealthMod,
                CreatureAutoAttackDPSMod = CreatureAutoAttackDPSMod,
                CreatureArmorMod = CreatureArmorMod,
                PlayerManaMod = PlayerManaMod,
                PlayerPrimaryStatMod = PlayerPrimaryStatMod,
                PlayerSecondaryStatMod = PlayerSecondaryStatMod,
                ArmorConstantMod = ArmorConstantMod,
                CreatureSpellDamageMod = CreatureSpellDamageMod,
            };
            return e;
        }
    }
}
