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

        public LobbyData(string nickName, ulong clientId, bool isReady, int points)
        {
            NickName = nickName;
            ClientId = clientId;
            IsReady = isReady;
            Points = points;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref NickName);
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref IsReady);
            serializer.SerializeValue(ref Points);
        }

        public bool Equals(LobbyData other)
        {
            return NickName.Equals(other.NickName) && ClientId == other.ClientId && IsReady == other.IsReady && Points == other.Points;
        }

        public override bool Equals(object obj)
        {
            return obj is LobbyData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NickName, ClientId, IsReady, Points);
        }
    }
}