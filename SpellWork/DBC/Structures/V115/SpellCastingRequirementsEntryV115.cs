using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class SpellCastingRequirementsEntryV115 : IConvertsTo<SpellCastingRequirementsEntry>
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        public int FacingCasterFlags;
        public ushort MinFactionID;
        public int MinReputation;
        public ushort RequiredAreasID;
        public byte RequiredAuraVision;
        public ushort RequiresSpellFocus;

        public SpellCastingRequirementsEntry ToCanonical()
        {
            var e = new SpellCastingRequirementsEntry
            {
                ID = (uint)ID,
                SpellID = SpellID,
                FacingCasterFlags = (byte)FacingCasterFlags,
                MinFactionID = MinFactionID,
                MinReputation = MinReputation,
                RequiredAreasID = RequiredAreasID,
                RequiredAuraVision = RequiredAuraVision,
                RequiresSpellFocus = RequiresSpellFocus,
            };
            return e;
        }
    }
}
