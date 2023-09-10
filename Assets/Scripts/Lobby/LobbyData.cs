using System;
using Misc;
using Unity.Netcode;

namespace Lobby
{
    [Serializable]
    public struct LobbyData : INetworkSerializable, IEquatable<LobbyData>
    {
        public NetworkString NickName;
        public ulong ClientId;
        public bool IsReady;
        public int Points;
        public int SkinIndex;
        public int BombSkinIndex;

        public LobbyData(string nickName, ulong clientId, bool isReady, int points, int skinIndex, int bombSkinIndex)
        {
            NickName = nickName;
            ClientId = clientId;
            IsReady = isReady;
            Points = points;
            SkinIndex = skinIndex;
            BombSkinIndex = bombSkinIndex;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref NickName);
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref IsReady);
            serializer.SerializeValue(ref Points);
            serializer.SerializeValue(ref SkinIndex);
            serializer.SerializeValue(ref BombSkinIndex);
        }

        public bool Equals(LobbyData other)
        {
            return NickName.Equals(other.NickName) && ClientId == other.ClientId && IsReady == other.IsReady && Points == other.Points && SkinIndex == other.SkinIndex && BombSkinIndex == other.BombSkinIndex;
        }

        public override bool Equals(object obj)
        {
            return obj is LobbyData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NickName, ClientId, IsReady, Points, SkinIndex, BombSkinIndex);
        }
    }
}