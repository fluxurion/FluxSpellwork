using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V12
{
    public sealed class SpellTargetRestrictionsEntryV12
    {
        [Index(true)]
        public uint ID;
        public short DifficultyID;
        public float ConeDegrees;
        public byte MaxTargets;
        public uint MaxTargetLevel;
        public short TargetCreatureType;
        public int Targets;
        public float Width;
        public int SpellID;
    }
}
