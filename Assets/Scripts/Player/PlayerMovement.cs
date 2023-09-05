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
        private NetworkVariable<float> _currentSpeed;
        private float _speedX, _speedZ;
        private GameStateController _gameStateController;

        [Inject]
        private void Inject(GameStateController gameStateController)
        {
            _gameStateController = gameStateController;
            print("INJECTED");
        }

        private void Awake()
        {
            NetworkManager.Singleton.NetworkTickSystem.Tick += OnTick;
            _canMove = new NetworkVariable<bool>();
            _currentSpeed = new NetworkVariable<float>(speed);
            _characterController = GetComponent<CharacterController>();
        }

        private void OnTick()
        {
            if (!IsOwner) return;
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            var direction = forward * _speedX + right * _speedZ;
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

        [ServerRpc(RequireOwnership = false)]
        public void IncreaseSpeedServerRpc(float amount)
        {
            _currentSpeed.Value += amount;
        }
    }
}