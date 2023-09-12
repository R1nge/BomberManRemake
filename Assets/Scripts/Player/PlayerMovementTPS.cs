using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerMovementTPS : PlayerMovement
    {
        [SerializeField] private Transform model;
        [SerializeField] private float rotationSpeed;
        private Vector3 _moveDirection = Vector3.zero;
        private float _curSpeedX, _curSpeedY;
        private bool _isFlipped;
        private CharacterController _characterController;
        private PlayerInput _playerInput;

        protected override void Awake()
        {
            base.Awake();
            NetworkManager.Singleton.NetworkTickSystem.Tick += OnTick;
            _characterController = GetComponent<CharacterController>();
            _playerInput = GetComponent<PlayerInput>();
        }

        [ClientRpc]
        public void SetFlippedClientRpc(bool isFlipped) => _isFlipped = isFlipped;

        public void OnMove(InputValue value)
        {
            if (_isFlipped)
            {
                _curSpeedX = -value.Get<Vector2>().y * CurrentSpeed;
                _curSpeedY = -value.Get<Vector2>().x * CurrentSpeed;
            }
            else
            {
                _curSpeedX = value.Get<Vector2>().y * CurrentSpeed;
                _curSpeedY = value.Get<Vector2>().x * CurrentSpeed;
            }
        }

        private void OnTick()
        {
            if (!IsOwner) return;
            _moveDirection = Vector3.forward * _curSpeedX + Vector3.right * _curSpeedY;
            Rotate();
            MoveServerRpc(_moveDirection);
        }

        [ServerRpc]
        private void MoveServerRpc(Vector3 direction)
        {
            _characterController.Move(direction);
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
    }
}