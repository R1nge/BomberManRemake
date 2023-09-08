using System;
using System.Collections.Generic;
using System.Linq;
using Misc;
using Unity.Netcode;
using UnityEngine;

namespace Lobby
{
    public class Lobby : NetworkBehaviour
    {
        public event Action<ulong> OnPlayerConnected;
        public event Action<ulong> OnPlayerDisconnected;

        public event Action<ulong, bool> OnReadyStateChanged;

        //TODO: use a dictionary?
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

        public bool CanStartGame()
        {
            bool everyoneIsReady = true;

            for (int i = 0; i < _players.Count; i++)
            {
                if (!_players[i].IsReady)
                {
                    everyoneIsReady = false;
                }
            }

            return everyoneIsReady;
        }

        public void AddPoints(ulong clientId, int amount)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i].ClientId == clientId)
                {
                    _players[i] = new LobbyData
                    {
                        ClientId = _players[i].ClientId,
                        IsReady = true,
                        NickName = _players[i].NickName,
                        Points = _players[i].Points + amount
                    };
                    break;
                }
            }
        }

        public void SortDescending()
        {
            var sortedList = new List<LobbyData>();

            for (int i = 0; i < _players.Count; i++)
            {
                sortedList.Add(_players[i]);
            }

            sortedList = sortedList.OrderByDescending(data => data.Points).ToList();

            _players.Clear();

            for (int i = 0; i < sortedList.Count; i++)
            {
                _players.Add(sortedList[i]);
            }
        }

        public int GetPlace(ulong clientId)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i].ClientId == clientId)
                {
                    return i;
                }
            }

            Debug.LogError("Player not found", this);

            return 99999;
        }

        private void Awake()
        {
            _players = new NetworkList<LobbyData>();
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
            if (NetworkManager.Singleton.LocalClientId == clientId)
            {
                CreatePlayerData(clientId);
            }

            OnPlayerConnected?.Invoke(clientId);
        }

        private void PlayerDisconnected(ulong clientId)
        {
            if (!NetworkManager.Singleton) return;
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
            CreatePlayerDataServerRpc(clientId, PlayerPrefs.GetString("Nick"));
        }

        [ServerRpc(RequireOwnership = false)]
        private void CreatePlayerDataServerRpc(ulong clientId, NetworkString nick)
        {
            var data = new LobbyData(nick, clientId, false, 0);
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

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= PlayerConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= PlayerDisconnected;
            }
        }
    }
}