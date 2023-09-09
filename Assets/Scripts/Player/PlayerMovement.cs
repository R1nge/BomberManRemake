using System;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public abstract class PlayerMovement : NetworkBehaviour
    {
        [SerializeField] private float speed;
        private NetworkVariable<float> _currentSpeed;

        protected float CurrentSpeed => _currentSpeed.Value;

        protected virtual void Awake() => _currentSpeed = new NetworkVariable<float>(speed);

        [ServerRpc(RequireOwnership = false)]
        public void IncreaseSpeedServerRpc(float amount) => _currentSpeed.Value += amount;
    }
}