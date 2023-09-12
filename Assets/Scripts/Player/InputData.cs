using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public struct InputData : INetworkSerializable
    {
        private Vector3 _movementDirection;

        public InputData(Vector3 movementDirection)
        {
            _movementDirection = movementDirection;
        }

        public Vector3 MovementDirection => _movementDirection;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _movementDirection);
        }
    }
}