using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class SpellRadiusEntryV115 : IConvertsTo<SpellRadiusEntry>
    {
        [Index(true)]
        public int ID;
        public float Radius;
        public float RadiusPerLevel;
        public float RadiusMin;
        public float RadiusMax;

        public SpellRadiusEntry ToCanonical()
        {
            var e = new SpellRadiusEntry
            {
                ID = (uint)ID,
                Radius = Radius,
                RadiusPerLevel = RadiusPerLevel,
                RadiusMin = RadiusMin,
                MaxRadius = RadiusMax,
            };
            return e;
        }
    }
}
