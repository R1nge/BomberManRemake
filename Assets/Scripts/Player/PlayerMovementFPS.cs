using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovementFPS : PlayerMovement
    {
        private CharacterController _characterController;
        private float _speedX, _speedZ;
        private PlayerInput _playerInput;
        private readonly Queue<InputData> _inputOnServer = new();
        private InputData _inputData;

        protected override void Awake()
        {
            base.Awake();
            NetworkManager.Singleton.NetworkTickSystem.Tick += OnTick;
            _characterController = GetComponent<CharacterController>();
            _playerInput = GetComponent<PlayerInput>();
        }

        private void Update()
        {
            if (!IsOwner) return;
            if (!_playerInput.InputEnabled) return;
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            var direction = forward * _speedX + right * _speedZ;
            _inputData = new InputData(direction);
            _characterController.Move(direction * Time.deltaTime);
        }

        private void OnTick()
        {
            if (!IsOwner) return;
            if (!_playerInput.InputEnabled) return;
            if (IsServer) return;
            SendDataServerRpc(_inputData);
            MoveServerRpc();
        }

        [ServerRpc(Delivery = RpcDelivery.Unreliable)]
        private void SendDataServerRpc(InputData inputData)
        {
            _inputOnServer.Enqueue(inputData);
        }

        [ServerRpc(Delivery = RpcDelivery.Unreliable)]
        private void MoveServerRpc()
        {
            if (_inputOnServer.Count <= 0) return;
            var inputData = _inputOnServer.Dequeue();
            _characterController.Move(inputData.MovementDirection);
        }

        public void OnMove(InputValue value)
        {
            if (!IsOwner) return;
            if (!_playerInput.InputEnabled) return;
            _speedX = value.Get<Vector2>().y * CurrentSpeed;
            _speedZ = value.Get<Vector2>().x * CurrentSpeed;
        }

        public override void OnDestroy()
        {
            if (!NetworkManager.Singleton) return;
            if (NetworkManager.Singleton.NetworkTickSystem == null) return;
            NetworkManager.Singleton.NetworkTickSystem.Tick -= OnTick;
        }
    }
}