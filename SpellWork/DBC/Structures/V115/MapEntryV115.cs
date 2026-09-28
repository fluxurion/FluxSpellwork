using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class MapEntryV115 : IConvertsTo<MapEntry>
    {
        [Index(true)]
        public int ID;
        public string Directory;
        public string MapName_lang;
        public string MapDescription0_lang;
        public string MapDescription1_lang;
        public string PvpShortDescription_lang;
        public string PvpLongDescription_lang;
        public byte MapType;
        public sbyte InstanceType;
        public byte ExpansionID;
        public ushort AreaTableID;
        public short LoadingScreenID;
        public short TimeOfDayOverride;
        public short ParentMapID;
        public short CosmeticParentMapID;
        public byte TimeOffset;
        public float MinimapIconScale;
        public int RaidOffset;
        public short CorpseMapID;
        public byte MaxPlayers;
        public short WindSettingsID;
        public int ZmpFileDataID;
        public int Field_1_15_4_56400_021;
        [Cardinality(3)]
        public int[] Flags = new int[3];

        public MapEntry ToCanonical()
        {
            var e = new MapEntry
            {
                ID = (uint)ID,
                Directory = Directory,
                MapName = MapName_lang,
                MapDescription0 = MapDescription0_lang,
                MapDescription1 = MapDescription1_lang,
                PvpShortDescription = PvpShortDescription_lang,
                PvpLongDescription = PvpLongDescription_lang,
                MapType = MapType,
                InstanceType = InstanceType,
                ExpansionID = ExpansionID,
                AreaTableID = AreaTableID,
                LoadingScreenID = LoadingScreenID,
                TimeOfDayOverride = TimeOfDayOverride,
                ParentMapID = ParentMapID,
                CosmeticParentMapID = CosmeticParentMapID,
                TimeOffset = TimeOffset,
                MinimapIconScale = MinimapIconScale,
                CorpseMapID = CorpseMapID,
                MaxPlayers = MaxPlayers,
                WindSettingsID = WindSettingsID,
                ZmpFileDataID = ZmpFileDataID,
                Flags = new uint[] { (uint)Flags[0], (uint)Flags[1], (uint)Flags[2] },
            };
            return e;
        }
    }
}
