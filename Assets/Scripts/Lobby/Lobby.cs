using System;
using System.Collections.Generic;
using System.Linq;
using Misc;
using Unity.Netcode;
using UnityEngine;

namespace Lobby
{
    public class Lobby
    {
        public event Action<ulong> OnPlayerConnected;
        public event Action<ulong> OnPlayerDisconnected;
        public event Action<ulong, bool> OnReadyStateChanged;

        private List<LobbyData> _players = new();

        public List<LobbyData> PlayerData => _players;

        public void ResetLobby()
        {
            _players = new List<LobbyData>();
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

            Debug.LogError("Lobby: Player data not found");

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
            _players = _players.OrderByDescending(data => data.Points).ToList();
        }

        public int? GetPlace(ulong clientId)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i].ClientId == clientId)
                {
                    return i;
                }
            }

            Debug.LogError("Lobby: Player not found");

            return null;
        }

        public void PlayerDisconnected(ulong clientId)
        {
            if (!NetworkManager.Singleton) return;
            if (!NetworkManager.Singleton.IsServer) return;
            var data = GetData(clientId);
            if (data != null)
            {
                _players?.Remove(data.Value);
            }

            OnPlayerDisconnected?.Invoke(clientId);
        }

        public void CreatePlayerData(ulong clientId, NetworkString nick, int skinIndex, int bombSkinIndex)
        {
            var data = new LobbyData(nick, clientId, false, 0, skinIndex, bombSkinIndex);
            _players.Add(data);
            Debug.Log($"Skin index: {data.SkinIndex}");
            Debug.Log($"Bomb skin index: {data.BombSkinIndex}");
            OnPlayerConnected?.Invoke(clientId);
        }

        public void ChangeReadyState(ulong clientId)
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

                    OnReadyStateChanged?.Invoke(clientId, isReady);
                }
            }
        }
    }
}