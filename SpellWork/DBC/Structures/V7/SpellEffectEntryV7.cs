using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V7
{
    public class SpellEffectEntryV7
    {
        [Index(true)]
        public uint ID;
        [Cardinality(4)]
        public uint[] EffectSpellClassMask = new uint[4];
        public int SpellID;
        public uint Effect;
        public uint EffectAura;
        public int EffectBasePoints;
        public uint EffectIndex;
        [Cardinality(2)]
        public int[] EffectMiscValues = new int[2];
        [Cardinality(2)]
        public uint[] EffectRadiusIndex = new uint[2];
        [Cardinality(2)]
        public uint[] ImplicitTarget = new uint[2];
        public uint DifficultyID;
        public float EffectAmplitude;
        public uint EffectAuraPeriod;
        public float EffectBonusCoefficient;
        public float EffectChainAmplitude;
        public uint EffectChainTargets;
        public int EffectDieSides;
        public uint EffectItemType;
        public uint EffectMechanic;
        public float EffectPointsPerResource;
        public float EffectRealPointsPerLevel;
        public uint EffectTriggerSpell;
        public float EffectPosFacing;
        public uint EffectAttributes;
        public float BonusCoefficientFromAP;
        public float PvPMultiplier;
    }
}
