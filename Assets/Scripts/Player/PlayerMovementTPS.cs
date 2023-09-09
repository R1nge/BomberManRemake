using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerMovementTPS : NetworkBehaviour
    {
        [SerializeField] private Transform model;
        [SerializeField] private float speed;
        [SerializeField] private float rotationSpeed;
        private Vector3 _moveDirection = Vector3.zero;
        private float _curSpeedX, _curSpeedY;
        private NetworkVariable<float> _currentSpeed;
        private CharacterController _characterController;
        private PlayerInput _playerInput;

        private void Awake()
        {
            NetworkManager.Singleton.NetworkTickSystem.Tick += OnTick;
            _characterController = GetComponent<CharacterController>();
            _playerInput = GetComponent<PlayerInput>();
            _currentSpeed = new NetworkVariable<float>(speed);
        }

        public void OnMove(InputValue value)
        {
            if (!_playerInput.InputEnabled) return;
            _curSpeedX = value.Get<Vector2>().y * _currentSpeed.Value;
            _curSpeedY = value.Get<Vector2>().x * _currentSpeed.Value;
        }

        private void OnTick()
        {
            if (!IsOwner || !_playerInput.InputEnabled) return;
            _moveDirection = Vector3.forward * _curSpeedX + Vector3.right * _curSpeedY;
            Rotate();
            _characterController.Move(_moveDirection);
        }

        private void Rotate()
        {
            if (_moveDirection != Vector3.zero)
            {
                var targetRot = Quaternion.LookRotation(_moveDirection, Vector3.up);
                model.transform.rotation =
                    Quaternion.RotateTowards(model.transform.rotation, targetRot, rotationSpeed);
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void IncreaseSpeedServerRpc(float amount) => _currentSpeed.Value += amount;
    }
}