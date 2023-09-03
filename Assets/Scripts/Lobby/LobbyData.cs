using System;
using Unity.Netcode;

namespace Lobby
{
    [Serializable]
    public struct LobbyData : INetworkSerializable, IEquatable<LobbyData>
    {
        public ulong ClientId;
        public bool IsReady;

        public LobbyData(ulong clientId, bool isReady)
        {
            ClientId = clientId;
            IsReady = isReady;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref IsReady);
        }

        public bool Equals(LobbyData other)
        {
            return ClientId == other.ClientId && IsReady == other.IsReady;
        }

        public override bool Equals(object obj)
        {
            return obj is LobbyData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ClientId, IsReady);
        }
    }
}