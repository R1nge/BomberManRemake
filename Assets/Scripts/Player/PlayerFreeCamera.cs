using Game;
using Game.StateMachines;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Player
{
    public class PlayerFreeCamera : NetworkBehaviour
    {
        [SerializeField] private new Camera camera;
        [SerializeField] private float sensitivity;
        [SerializeField] private float limitX;
        private float _rotationX, _rotationY;
        private GameStateController2 _gameStateController2;

        public void Disable()
        {
            camera.enabled = false;
            enabled = false;
        }

        [Inject]
        private void Inject(GameStateController2 gameStateController) => _gameStateController2 = gameStateController;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _gameStateController2.OnStateChanged += StateChanged;
            }
        }

        private void StateChanged(GameStates newState)
        {
            switch (newState)
            {
                case GameStates.NextRound:
                    Destroy();
                    break;
            }
        }

        private void Destroy() => NetworkObject.Despawn(true);

        public void OnLook(InputValue value)
        {
            if (!IsOwner) return;
            _rotationX += -value.Get<Vector2>().y * sensitivity;
            _rotationX = Mathf.Clamp(_rotationX, -limitX, limitX);
            _rotationY = value.Get<Vector2>().x * sensitivity;
            Rotate();
        }

        private void Rotate()
        {
            if (!IsOwner) return;
            transform.rotation *= Quaternion.Euler(0, _rotationY, 0);
            camera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
        }

        public override void OnDestroy()
        {
            if (IsServer)
            {
                _gameStateController2.OnStateChanged -= StateChanged;
            }
        }
    }
}