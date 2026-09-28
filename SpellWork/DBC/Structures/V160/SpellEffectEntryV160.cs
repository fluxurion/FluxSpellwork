using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V160
{
    public sealed class SpellEffectEntryV160 : IConvertsTo<SpellEffectEntry>
    {
        [Index(true)]
        public int ID;
        public short EffectAura;
        public short DifficultyID;
        public int EffectIndex;
        public uint Effect;
        public float EffectAmplitude;
        public int EffectAttributes;
        public int EffectAuraPeriod;
        public float EffectBonusCoefficient;
        public float EffectChainAmplitude;
        public int EffectChainTargets;
        public int EffectItemType;
        public int EffectMechanic;
        public float EffectPointsPerResource;
        public float EffectPos_facing;
        public float EffectRealPointsPerLevel;
        public int EffectTriggerSpell;
        public float BonusCoefficientFromAP;
        public float PvpMultiplier;
        public float Coefficient;
        public float Variance;
        public float ResourceCoefficient;
        public float GroupSizeBasePointsCoefficient;
        public float EffectBasePointsF;
        public int ScalingClass;
        public int Node__Field_12_0_0_63534_001;
        [Cardinality(2)]
        public int[] EffectMiscValue = new int[2];
        [Cardinality(2)]
        public uint[] EffectRadiusIndex = new uint[2];
        [Cardinality(4)]
        public int[] EffectSpellClassMask = new int[4];
        [Cardinality(2)]
        public short[] ImplicitTarget = new short[2];
        public int SpellID;

        public SpellEffectEntry ToCanonical()
        {
            var e = new SpellEffectEntry
            {
                ID = (uint)ID,
                EffectAura = EffectAura,
                DifficultyID = (int)DifficultyID,
                EffectIndex = EffectIndex,
                Effect = (int)Effect,
                EffectAmplitude = EffectAmplitude,
                EffectAttributes = EffectAttributes,
                EffectAuraPeriod = EffectAuraPeriod,
                EffectBonusCoefficient = EffectBonusCoefficient,
                EffectChainAmplitude = EffectChainAmplitude,
                EffectChainTargets = EffectChainTargets,
                EffectItemType = EffectItemType,
                EffectMechanic = EffectMechanic,
                EffectPointsPerResource = EffectPointsPerResource,
                EffectPosFacing = EffectPos_facing,
                EffectRealPointsPerLevel = EffectRealPointsPerLevel,
                EffectTriggerSpell = EffectTriggerSpell,
                BonusCoefficientFromAP = BonusCoefficientFromAP,
                PvpMultiplier = PvpMultiplier,
                Coefficient = Coefficient,
                Variance = Variance,
                ResourceCoefficient = ResourceCoefficient,
                GroupSizeBasePointsCoefficient = GroupSizeBasePointsCoefficient,
                EffectBasePoints = EffectBasePointsF,
                ScalingClass = ScalingClass,
                EffectMiscValue = EffectMiscValue,
                EffectRadiusIndex = EffectRadiusIndex,
                EffectSpellClassMask = EffectSpellClassMask,
                ImplicitTarget = ImplicitTarget,
                SpellID = SpellID,
            };
            return e;
        }
    }
}
