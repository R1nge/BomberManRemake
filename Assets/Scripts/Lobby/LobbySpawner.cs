using System.Collections.Generic;
using Skins;
using Skins.Players;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Lobby
{
    public class LobbySpawner : NetworkBehaviour
    {
        [SerializeField] private Transform[] positions;
        private List<PlayerModel> _playerModels;
        private int _lastIndex;
        private DiContainer _diContainer;
        private Lobby _lobby;
        private SkinManager _skinManager;

        [Inject]
        private void Inject(DiContainer diContainer, Lobby lobby, SkinManager skinManager)
        {
            _diContainer = diContainer;
            _lobby = lobby;
            _skinManager = skinManager;
        }

        private void Awake()
        {
            _playerModels ??= new List<PlayerModel>();
            _lobby.OnPlayerConnected += SpawnPlayer;
            _lobby.OnPlayerConnected += UpdateUIForClients;
            _lobby.OnPlayerDisconnected += DestroyPlayer;
            _lobby.OnReadyStateChanged += ChangeReadyState;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            SpawnPlayerSkinServerRpc(0);
        }

        private void SpawnPlayer(ulong clientId) => SpawnPlayerSkinServerRpc(clientId);

        private void UpdateUIForClients(ulong clientId)
        {
            if (!IsServer) return;

            var index = 0;

            foreach (var data in _lobby.PlayerData)
            {
                var value = data.Value;

                _playerModels[index].UpdateUIServerRpc(value.NickName, value.IsReady);

                index++;
            }
        }

        private void ChangeReadyState(ulong clientId, bool isReady)
        {
            if (!IsServer) return;

            var index = 0;

            foreach (var data in _lobby.PlayerData)
            {
                var value = data.Value;

                if (_playerModels[index].OwnerClientId == clientId)
                {
                    _playerModels[index].UpdateUIServerRpc(value.NickName, isReady);
                }

                index++;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnPlayerSkinServerRpc(ulong clientId)
        {
            var data = _lobby.GetData(clientId);
            var model = _diContainer.InstantiatePrefabForComponent<PlayerModel>
            (
                _skinManager.GetLobby(data.Value.SkinIndex),
                positions[_lastIndex].position,
                Quaternion.identity,
                null
            );

            model.GetComponent<NetworkObject>().SpawnWithOwnership(clientId, true);
            _lastIndex++;
            _playerModels.Add(model);
            ChangeReadyState(clientId, false);
        }

        private void DestroyPlayer(ulong clientId)
        {
            if (!IsServer) return;

            _lastIndex--;
            for (int i = 0; i < _playerModels.Count; i++)
            {
                if (_playerModels[i].OwnerClientId == clientId)
                {
                    _playerModels.RemoveAt(i);
                }
            }

            UpdateUIForClients(clientId);
        }

        public override void OnDestroy()
        {
            _lobby.OnPlayerConnected -= SpawnPlayer;
            _lobby.OnPlayerConnected -= UpdateUIForClients;
            _lobby.OnPlayerDisconnected -= DestroyPlayer;
            _lobby.OnReadyStateChanged -= ChangeReadyState;
            base.OnDestroy();
        }
    }
}