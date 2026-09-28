using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class SpellTargetRestrictionsEntryV115 : IConvertsTo<SpellTargetRestrictionsEntry>
    {
        [Index(true)]
        public int ID;
        public short DifficultyID;
        public float ConeDegrees;
        public byte MaxTargets;
        public uint MaxTargetLevel;
        public short TargetCreatureType;
        public int Targets;
        public float Width;
        public int SpellID;

        public SpellTargetRestrictionsEntry ToCanonical()
        {
            var e = new SpellTargetRestrictionsEntry
            {
                ID = (uint)ID,
                DifficultyID = (byte)DifficultyID,
                ConeDegrees = ConeDegrees,
                MaxTargets = MaxTargets,
                MaxTargetLevel = MaxTargetLevel,
                TargetCreatureType = TargetCreatureType,
                Targets = Targets,
                Width = Width,
                SpellID = SpellID,
            };
            return e;
        }
    }
}
