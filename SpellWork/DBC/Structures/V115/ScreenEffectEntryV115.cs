using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V115
{
    public sealed class ScreenEffectEntryV115 : IConvertsTo<ScreenEffectEntry>
    {
        [Index(true)]
        public int ID;
        public string Name;
        [Cardinality(4)]
        public int[] Param = new int[4];
        public sbyte Effect;
        public uint FullScreenEffectID;
        public ushort LightParamsID;
        public ushort LightParamsFadeIn;
        public ushort LightParamsFadeOut;
        public uint SoundAmbienceID;
        public uint ZoneMusicID;
        public short TimeOfDayOverride;
        public sbyte EffectMask;
        public int LightFlags;

        public ScreenEffectEntry ToCanonical()
        {
            var e = new ScreenEffectEntry
            {
                ID = (uint)ID,
                Name = Name,
                Param = Param,
                Effect = Effect,
                FullScreenEffectID = FullScreenEffectID,
                LightParamsID = LightParamsID,
                LightParamsFadeIn = LightParamsFadeIn,
                LightParamsFadeOut = LightParamsFadeOut,
                SoundAmbienceID = SoundAmbienceID,
                ZoneMusicID = ZoneMusicID,
                TimeOfDayOverride = TimeOfDayOverride,
                EffectMask = EffectMask,
                LightFlags = (byte)LightFlags,
            };
            return e;
        }
    }
}
