﻿using System;
using Unity.Netcode;
using UnityEngine;

namespace Lobby
{
    public class Lobby : NetworkBehaviour
    {
        public event Action<ulong> OnPlayerConnected;
        public event Action<ulong> OnPlayerDisconnected;
        public event Action<ulong, bool> OnReadyStateChanged;
        private NetworkList<LobbyData> _players;

        public NetworkList<LobbyData> PlayerData => _players;

        public LobbyData? GetData(ulong clientId)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i].ClientId == clientId)
                {
                    return _players[i];
                }
            }

            Debug.LogError("Player data not found", this);

            return null;
        }

        private void Awake()
        {
            _players ??= new NetworkList<LobbyData>();
            NetworkManager.Singleton.OnClientConnectedCallback += PlayerConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += PlayerDisconnected;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            PlayerConnected(0);
        }

        private void PlayerConnected(ulong clientId)
        {
            CreatePlayerData(clientId);
            OnPlayerConnected?.Invoke(clientId);
        }

        private void PlayerDisconnected(ulong clientId)
        {
            if (!IsServer) return;
            var data = GetData(clientId);
            if (data != null)
            {
                _players.Remove(data.Value);
            }

            OnPlayerDisconnected?.Invoke(clientId);
        }

        private void CreatePlayerData(ulong clientId)
        {
            if (!IsServer) return;
            var data = new LobbyData(PlayerPrefs.GetString("Nick"), clientId, false);
            _players.Add(data);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ChangeReadyStateServerRpc(ulong clientId)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i].ClientId == clientId)
                {
                    _players[i] = new LobbyData
                    {
                        NickName = _players[i].NickName,
                        ClientId = clientId,
                        IsReady = !_players[i].IsReady
                    };

                    var isReady = _players[i].IsReady;

                    ChangeReadyStateClientRpc(clientId, isReady);
                    break;
                }
            }
        }

        [ClientRpc]
        private void ChangeReadyStateClientRpc(ulong clientId, bool ready)
        {
            OnReadyStateChanged?.Invoke(clientId, ready);
        }
    }
}