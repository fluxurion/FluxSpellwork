using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class SpellProcsPerMinuteEntryV115 : IConvertsTo<SpellProcsPerMinuteEntry>
    {
        [Index(true)]
        public int ID;
        public float BaseProcRate;
        public int Flags;

        public SpellProcsPerMinuteEntry ToCanonical()
        {
            var e = new SpellProcsPerMinuteEntry
            {
                ID = (uint)ID,
                BaseProcRate = BaseProcRate,
                Flags = (byte)Flags,
            };
            return e;
        }
    }
}
