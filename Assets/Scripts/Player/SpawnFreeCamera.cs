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

        private void Spawn()
        {
            if (!IsOwner) return;
            SpawnServerRpc();
        }

        [ServerRpc]
        private void SpawnServerRpc(ServerRpcParams rpcParams = default)
        {
            var targetId = new ClientRpcParams()
            {
                Send = new ClientRpcSendParams()
                {
                    TargetClientIds = new[] { rpcParams.Receive.SenderClientId }
                }
            };

            SpawnClientRpc(targetId);
        }

        [ClientRpc]
        private void SpawnClientRpc(ClientRpcParams rpcParams)
        {
            if (IsOwner)
            {
                _diContainer.InstantiatePrefabForComponent<PlayerFreeCamera>(freeCamera, transform.position,
                    Quaternion.identity, null);
            }
        }

        public override void OnDestroy() => _playerHealth.OnDeath -= Spawn;
    }
}