using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V12
{
    public sealed class AreaGroupMemberEntryV12
    {
        [Index(true)]
        public uint ID;
        public ushort AreaID;
        public int AreaGroupID;
    }
}
