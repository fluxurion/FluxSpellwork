using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V7
{
    public sealed class SpellPowerEntryV7
    {
        [Index(true)]
        public int ID;
        public uint SpellID;
        public uint ManaCost;
        public float ManaCostPercentage;
        public float ManaCostPercentagePerSecond;
        public uint RequiredAura;
        public float HealthCostPercentage;
        public byte PowerIndex;
        public byte PowerType;
        public uint ManaCostPerLevel;
        public uint ManaCostPerSecond;
        public uint ManaCostAdditional;
        public uint PowerDisplayID;
        public uint UnitPowerBarID;
    }
}
