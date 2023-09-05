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

        public LobbyData(string nickName, ulong clientId, bool isReady)
        {
            NickName = nickName;
            ClientId = clientId;
            IsReady = isReady;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref NickName);
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref IsReady);
        }

        public bool Equals(LobbyData other)
        {
            return NickName == other.NickName && ClientId == other.ClientId && IsReady == other.IsReady;
        }

        public override bool Equals(object obj)
        {
            return obj is LobbyData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NickName, ClientId, IsReady);
        }
    }
}