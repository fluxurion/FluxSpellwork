using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V160
{
    public sealed class AreaTableEntryV160 : IConvertsTo<AreaTableEntry>
    {
        [Index(true)]
        public int ID;
        public string ZoneName;
        public string AreaName_lang;
        public ushort ContinentID;
        public ushort ParentAreaID;
        public short AreaBit;
        public byte SoundProviderPref;
        public byte SoundProviderPrefUnderwater;
        public ushort AmbienceID;
        public ushort UwAmbience;
        public ushort ZoneMusic;
        public ushort UwZoneMusic;
        public sbyte ExplorationLevel;
        public ushort IntroSound;
        public uint UwIntroSound;
        public byte FactionGroupMask;
        public float Ambient_multiplier;
        public int MountFlags;
        public int PvpCombatWorldStateID;
        public byte WildBattlePetLevelMin;
        public byte WildBattlePetLevelMax;
        public byte WindSettingsID;
        public int ContentTuningID;
        [Cardinality(2)]
        public int[] Flags = new int[2];
        [Cardinality(4)]
        public ushort[] LiquidTypeID = new ushort[4];

        public AreaTableEntry ToCanonical()
        {
            var e = new AreaTableEntry
            {
                ID = (uint)ID,
                ZoneName = ZoneName,
                AreaName = AreaName_lang,
                ContinentID = ContinentID,
                ParentAreaID = ParentAreaID,
                AreaBit = AreaBit,
                SoundProviderPref = SoundProviderPref,
                SoundProviderPrefUnderwater = SoundProviderPrefUnderwater,
                AmbienceID = AmbienceID,
                UwAmbience = UwAmbience,
                ZoneMusic = ZoneMusic,
                UwZoneMusic = UwZoneMusic,
                IntroSound = IntroSound,
                UwIntroSound = UwIntroSound,
                FactionGroupMask = FactionGroupMask,
                AmbientMultiplier = Ambient_multiplier,
                MountFlags = (byte)MountFlags,
                PvpCombatWorldStateID = (short)PvpCombatWorldStateID,
                WildBattlePetLevelMin = WildBattlePetLevelMin,
                WildBattlePetLevelMax = WildBattlePetLevelMax,
                WindSettingsID = WindSettingsID,
                ContentTuningID = ContentTuningID,
                Flags = Flags,
                LiquidTypeID = LiquidTypeID,
            };
            return e;
        }
    }
}
