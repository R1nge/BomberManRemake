using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Lobby
{
    public class LobbySpawner : NetworkBehaviour
    {
        [SerializeField] private GameObject playerModel;
        [SerializeField] private Transform[] positions;
        private int _lastIndex;
        private DiContainer _diContainer;
        private Lobby _lobby;

        [Inject]
        private void Inject(DiContainer diContainer, Lobby lobby)
        {
            _diContainer = diContainer;
            _lobby = lobby;
        }

        public override void OnNetworkSpawn()
        {
            _lobby.OnPlayerConnected += SpawnPlayer;
            _lobby.OnPlayerDisconnected += DestroyPlayer;
            SpawnPlayer(0);
        }

        private void SpawnPlayer(ulong clientId)
        {
            if(!IsServer) return;
            
            var model = _diContainer.InstantiatePrefab
            (
                playerModel,
                positions[_lastIndex].position,
                Quaternion.identity,
                null
            );
            
            model.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            _lastIndex++;
        }

        private void DestroyPlayer()
        {
            _lastIndex--;
        }
    }
}