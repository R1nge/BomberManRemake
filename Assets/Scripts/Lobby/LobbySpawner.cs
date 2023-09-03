using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Lobby
{
    public class LobbySpawner : NetworkBehaviour
    {
        [SerializeField] private PlayerModel playerModel;
        [SerializeField] private Transform[] positions;
        private List<PlayerModel> _playerModels;
        private int _lastIndex;
        private DiContainer _diContainer;
        private Lobby _lobby;

        [Inject]
        private void Inject(DiContainer diContainer, Lobby lobby)
        {
            _diContainer = diContainer;
            _lobby = lobby;
        }

        private void Awake() => _playerModels = new List<PlayerModel>();

        public override void OnNetworkSpawn()
        {
            _lobby.OnPlayerConnected += SpawnPlayer;
            _lobby.OnPlayerConnected += UpdateUIForClients;
            _lobby.OnPlayerDisconnected += DestroyPlayer;
            _lobby.OnReadyStateChanged += ChangeReadyState;
            SpawnPlayer(0);
        }

        private void UpdateUIForClients(ulong obj)
        {
            if (!IsServer) return;
            
            for (int i = 0; i < _playerModels.Count; i++)
            {
                _playerModels[i].UpdateUIServerRpc(_lobby.PlayerData[i].IsReady);
            }
        }

        private void ChangeReadyState(ulong clientId, bool isReady)
        {
            if (!IsServer) return;

            for (int i = 0; i < _playerModels.Count; i++)
            {
                if (_playerModels[i].OwnerClientId == clientId)
                {
                    _playerModels[i].UpdateUIServerRpc(isReady);
                }
            }
        }

        private void SpawnPlayer(ulong clientId)
        {
            if (!IsServer) return;

            var model = _diContainer.InstantiatePrefabForComponent<PlayerModel>
            (
                playerModel,
                positions[_lastIndex].position,
                Quaternion.identity,
                null
            );

            model.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            _lastIndex++;
            _playerModels.Add(model);
            ChangeReadyState(clientId, false);
        }

        private void DestroyPlayer()
        {
            _lastIndex--;
        }
    }
}