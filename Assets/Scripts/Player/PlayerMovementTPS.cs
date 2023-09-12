using System.Collections.Generic;
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
        private readonly Queue<InputData> _inputOnServer = new();

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
            if (!_playerInput.InputEnabled) return;
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
            if (!_playerInput.InputEnabled) return;
            _moveDirection = Vector3.forward * _curSpeedX + Vector3.right * _curSpeedY;
            var inputData = new InputData(_moveDirection);
            _characterController.Move(_moveDirection);
            Rotate();

            if (!IsServer)
            {
                SendDataServerRpc(inputData);
                MoveServerRpc();
            }
        }
        
        [ServerRpc]
        private void SendDataServerRpc(InputData inputData)
        {
            _inputOnServer.Enqueue(inputData);
        }

        [ServerRpc]
        private void MoveServerRpc()
        {
            if (_inputOnServer.Count <= 0) return;
            var inputData = _inputOnServer.Dequeue();
            _characterController.Move(inputData.MovementDirection);
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