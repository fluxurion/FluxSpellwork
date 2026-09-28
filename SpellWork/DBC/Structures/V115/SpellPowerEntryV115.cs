using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class SpellPowerEntryV115 : IConvertsTo<SpellPowerEntry>
    {
        [Index(false)]
        public int ID;
        public byte OrderIndex;
        public int ManaCost;
        public int ManaCostPerLevel;
        public int ManaPerSecond;
        public uint PowerDisplayID;
        public int AltPowerBarID;
        public float PowerCostPct;
        public float PowerCostMaxPct;
        public float PowerPctPerSecond;
        public sbyte PowerType;
        public int RequiredAuraSpellID;
        public uint OptionalCost;
        public int SpellID;

        public SpellPowerEntry ToCanonical()
        {
            var e = new SpellPowerEntry
            {
                ID = ID,
                OrderIndex = OrderIndex,
                ManaCost = ManaCost,
                ManaCostPerLevel = ManaCostPerLevel,
                ManaPerSecond = ManaPerSecond,
                PowerDisplayID = PowerDisplayID,
                AltPowerBarID = AltPowerBarID,
                PowerCostPct = PowerCostPct,
                PowerCostMaxPct = PowerCostMaxPct,
                PowerPctPerSecond = PowerPctPerSecond,
                PowerType = PowerType,
                RequiredAuraSpellID = RequiredAuraSpellID,
                OptionalCost = OptionalCost,
                SpellID = SpellID,
            };
            return e;
        }
    }
}
