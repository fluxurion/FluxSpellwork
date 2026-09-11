using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V12
{
    public class SpellCastingRequirementsEntryV12
    {
        [Index(true)]
        public uint ID;
        public int SpellID;
        public int FacingCasterFlags;
        public ushort MinFactionID;
        public int MinReputation;
        public ushort RequiredAreasID;
        public byte RequiredAuraVision;
        public ushort RequiresSpellFocus;
    }
}
