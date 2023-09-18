using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public abstract class PlayerMovement : NetworkBehaviour
    {
        [SerializeField] private float speed, maxSpeed;
        private NetworkVariable<float> _currentSpeed;
        protected PlayerAnimator PlayerAnimator;

        protected float CurrentSpeed => _currentSpeed.Value;

        protected virtual void Awake()
        {
            _currentSpeed = new NetworkVariable<float>(speed);
            PlayerAnimator = GetComponent<PlayerAnimator>();
        }

        [ServerRpc(RequireOwnership = false)]
        public void IncreaseSpeedServerRpc(float amount)
        {
            _currentSpeed.Value = Mathf.Clamp(_currentSpeed.Value + amount, 0, maxSpeed);
        }
    }
}