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

        protected override void Awake()
        {
            base.Awake();
            NetworkManager.Singleton.NetworkTickSystem.Tick += OnTick;
            _characterController = GetComponent<CharacterController>();
            _playerInput = GetComponent<PlayerInput>();
        }

        private void OnTick()
        {
            if (!IsOwner) return;
            if (!_playerInput.InputEnabled) return;
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            var direction = forward * _speedX + right * _speedZ;
            _characterController.Move(direction);
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