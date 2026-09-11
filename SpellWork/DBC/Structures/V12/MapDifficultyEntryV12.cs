using DBFileReaderLib.Attributes;
using System;

namespace SpellWork.DBC.Structures.V12
{
    public class MapDifficultyEntryV12 : IComparable
    {
        public string Message;
        [Index(true)]
        public uint ID;
        public short DifficultyID;
        public int LockID;
        public byte ResetInterval;
        public int MaxPlayers;
        public byte ItemContext;
        public int ItemContextPickerID;
        public int Flags;
        public int ContentTuningID;
        public int WorldStateExpressionID;
        public int MapID;

        public int CompareTo(object obj)
        {
            return obj is MapDifficultyEntryV12 m ? ID.CompareTo(m.ID) : 1;
        }
    }
}
