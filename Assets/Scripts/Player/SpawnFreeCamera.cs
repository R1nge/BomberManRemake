using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Player
{
    public class SpawnFreeCamera : NetworkBehaviour
    {
        [SerializeField] private PlayerFreeCamera freeCamera;
        private PlayerHealth _playerHealth;
        private DiContainer _diContainer;

        [Inject]
        private void Inject(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        private void Awake()
        {
            _playerHealth = GetComponent<PlayerHealth>();
            _playerHealth.OnDeath += Spawn;
        }

        private void Spawn() => SpawnServerRpc();

        [ServerRpc(RequireOwnership = false)]
        private void SpawnServerRpc(ServerRpcParams rpcParams = default)
        {
            print("SPAWN");
            var spectator = _diContainer.InstantiatePrefabForComponent<PlayerFreeCamera>(freeCamera, transform.position,
                Quaternion.identity, null);
            var netSpectator = spectator.GetComponent<NetworkObject>();
            netSpectator.SpawnWithOwnership(rpcParams.Receive.SenderClientId, true);
            DisableOnNonOwnersClientRpc(netSpectator);
        }

        [ClientRpc]
        private void DisableOnNonOwnersClientRpc(NetworkObjectReference spectator)
        {
            if (IsOwner) return;
            if (spectator.TryGet(out NetworkObject networkObject))
            {
                if (networkObject.TryGetComponent(out PlayerFreeCamera freeCamera))
                {
                    freeCamera.Disable();
                }

                if (networkObject.TryGetComponent(out FreeCameraMovement freeCameraMovement))
                {
                    freeCameraMovement.Disable();
                }
            }
        }

        public override void OnDestroy() => _playerHealth.OnDeath -= Spawn;
    }
}