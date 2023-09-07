using Game;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Player
{
    public class PlayerFreeCamera : NetworkBehaviour
    {
        [SerializeField] private new Camera camera;
        [SerializeField] private float sensitivity;
        [SerializeField] private float limitX;
        private float _rotationX;
        private RoundManager _roundManager;

        public void Disable()
        {
            camera.enabled = false;
            enabled = false;
        }

        [Inject]
        private void Inject(RoundManager roundManager) => _roundManager = roundManager;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _roundManager.OnCleanUpBeforeNextRound += Destroy;
            }
        }

        private void Destroy() => NetworkObject.Despawn(true);

        private void Update()
        {
            _rotationX += -Input.GetAxis("Mouse Y") * sensitivity;
            _rotationX = Mathf.Clamp(_rotationX, -limitX, limitX);
            camera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * sensitivity, 0);
        }

        public override void OnDestroy()
        {
            if (IsServer)
            {
                _roundManager.OnCleanUpBeforeNextRound -= Destroy;
            }
        }
    }
}