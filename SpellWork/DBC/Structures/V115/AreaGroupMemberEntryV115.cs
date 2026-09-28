using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class AreaGroupMemberEntryV115 : IConvertsTo<AreaGroupMemberEntry>
    {
        [Index(true)]
        public int ID;
        public ushort AreaID;
        public int AreaGroupID;

        public AreaGroupMemberEntry ToCanonical()
        {
            var e = new AreaGroupMemberEntry
            {
                ID = (uint)ID,
                AreaID = AreaID,
                AreaGroupID = (ushort)AreaGroupID,
            };
            return e;
        }
    }
}
