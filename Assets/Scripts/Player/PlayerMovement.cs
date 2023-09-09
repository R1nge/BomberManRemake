using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : NetworkBehaviour
    {
        [SerializeField] private float speed;
        private CharacterController _characterController;
        private NetworkVariable<float> _currentSpeed;
        private float _speedX, _speedZ;
        private PlayerInput _playerInput;

        private void Awake()
        {
            NetworkManager.Singleton.NetworkTickSystem.Tick += OnTick;
            _currentSpeed = new NetworkVariable<float>(speed);
            _characterController = GetComponent<CharacterController>();
            _playerInput = GetComponent<PlayerInput>();
        }

        private void OnTick()
        {
            if (!IsOwner) return;
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            var direction = forward * _speedX + right * _speedZ;
            _characterController.Move(direction);
        }

        public void OnMove(InputValue value)
        {
            if (!IsOwner) return;
            if (!_playerInput.InputEnabled) return;
            _speedX = value.Get<Vector2>().y * speed;
            _speedZ = value.Get<Vector2>().x * speed;
        }

        [ServerRpc(RequireOwnership = false)]
        public void IncreaseSpeedServerRpc(float amount) => _currentSpeed.Value += amount;

        public override void OnDestroy()
        {
            if (!NetworkManager.Singleton) return;
            if (NetworkManager.Singleton.NetworkTickSystem == null) return;
            NetworkManager.Singleton.NetworkTickSystem.Tick -= OnTick;
        }
    }
}