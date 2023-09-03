using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Game
{
    public class PlayerSpawner : NetworkBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        private DiContainer _diContainer;

        [Inject]
        private void Inject(DiContainer diContainer) => _diContainer = diContainer;

        public override void OnNetworkSpawn() => SpawnServerRpc();

        [ServerRpc(RequireOwnership = false)]
        private void SpawnServerRpc(ServerRpcParams rpcParams = default)
        {
            var player = _diContainer.InstantiatePrefab(playerPrefab);
            player.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId);
        }
    }
}