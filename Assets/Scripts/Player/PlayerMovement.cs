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
        [SerializeField] private float speed;
        private CharacterController _characterController;
        private NetworkVariable<bool> _canMove;
        private float _speedX, _speedZ;
        private GameStateControllerView _gameStateController;

        [Inject]
        private void Inject(GameStateControllerView gameStateController)
        {
            _gameStateController = gameStateController;
            print("INJECTED");
        }

        private void Awake()
        {
            NetworkManager.Singleton.NetworkTickSystem.Tick += OnTick;
            _canMove = new NetworkVariable<bool>();
            _characterController = GetComponent<CharacterController>();
        }

        private void OnTick()
        {
            if (!IsOwner) return;
            var direction = Vector3.forward * _speedX + Vector3.right * _speedZ;
            MoveServerRpc(direction);
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
            _speedX = value.Get<Vector2>().y * speed;
            _speedZ = value.Get<Vector2>().x * speed;
        }

        [ServerRpc]
        private void MoveServerRpc(Vector3 direction)
        {
            _characterController.Move(direction);
        }
    }
}