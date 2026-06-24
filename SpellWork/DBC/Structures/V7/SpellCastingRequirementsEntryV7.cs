using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V7
{
    public class SpellCastingRequirementsEntryV7
    {
        [Index(true)]
        public int ID;
        public int SpellID;
        public ushort MinFactionID;
        public ushort RequiredAreasID;
        public ushort RequiresSpellFocus;
        public byte FacingCasterFlags;
        public byte MinReputation;
        public byte RequiredAuraVision;
    }
}
