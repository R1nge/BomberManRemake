using System;
using System.Collections.Generic;
using System.Linq;
using Misc;
using Skins.Bombs;
using Skins.Players;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Lobby
{
    public class Lobby : NetworkBehaviour
    {
        public event Action<ulong> OnPlayerConnected;
        public event Action<ulong> OnPlayerDisconnected;
        public event Action<ulong, bool> OnReadyStateChanged;

        //TODO: use a dictionary?
        private NetworkList<LobbyData> _players;
        private BombSkinManager _bombSkinManager;
        private SkinManager _skinManager;
        private PlayFabManager _playFabManager;
        public NetworkList<LobbyData> PlayerData => _players;

        [Inject]
        private void Inject(SkinManager skinManager, BombSkinManager bombSkinManager, PlayFabManager playFabManager)
        {
            _skinManager = skinManager;
            _bombSkinManager = bombSkinManager;
            _playFabManager = playFabManager;
        }

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
            if (_players.Count <= 1)
            {
                return false;
            }
            
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
                        Points = _players[i].Points + amount,
                        SkinIndex = _players[i].SkinIndex,
                        BombSkinIndex = _players[i].BombSkinIndex
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
            CreatePlayerDataServerRpc(clientId, _playFabManager.GetUserName, _skinManager.SkinIndex,
                _bombSkinManager.SkinIndex);
        }

        [ServerRpc(RequireOwnership = false)]
        private void CreatePlayerDataServerRpc(ulong clientId, NetworkString nick, int skinIndex, int bombSkinIndex)
        {
            var data = new LobbyData(nick, clientId, false, 0, skinIndex, bombSkinIndex);
            _players.Add(data);
            print($"Skin index: {data.SkinIndex}");
            print($"Bomb skin index: {data.BombSkinIndex}");
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
                        IsReady = !_players[i].IsReady,
                        BombSkinIndex = _players[i].BombSkinIndex,
                        Points = _players[i].Points,
                        SkinIndex = _players[i].SkinIndex
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