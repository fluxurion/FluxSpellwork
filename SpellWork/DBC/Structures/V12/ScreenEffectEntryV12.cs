using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures.V12
{
    public sealed class ScreenEffectEntryV12
    {
        [Index(true)]
        public uint ID;
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
    }
}
