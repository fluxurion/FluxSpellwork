using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V7
{
    public sealed class SpellTargetRestrictionsEntryV7
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        public float ConeAngle;
        public float Width;
        public uint Targets;
        public ushort TargetCreatureType;
        public byte DifficultyID;
        public byte MaxAffectedTargets;
        public uint MaxTargetLevel;
    }
}
