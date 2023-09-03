using System;
using System.Collections;
using Game;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : NetworkBehaviour
    {
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private float speed;
        private CharacterController _characterController;
        private NetworkVariable<bool> _canMove;
        private GameStateControllerView _gameStateController;

        [Inject]
        private void Inject(GameStateControllerView gameStateController)
        {
            _gameStateController = gameStateController;
            print("INJECTED");
        }

        private void Awake()
        {
            _canMove = new NetworkVariable<bool>();
            _characterController = GetComponent<CharacterController>();
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _gameStateController.OnGameStarted += EnableMovementServerRpc;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void EnableMovementServerRpc()
        {
            _canMove.Value = true;
            print("ENABLED MOVEMENT");
        }

        public void OnMove(InputValue value)
        {
            if (!IsOwner) return;
            if (!_canMove.Value) return;
            MoveServerRpc(value.Get<Vector2>());
        }

        [ServerRpc]
        private void MoveServerRpc(Vector3 direction)
        {
            transform.position += direction;
        }
    }
}