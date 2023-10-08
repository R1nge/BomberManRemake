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

        private Dictionary<ulong, LobbyData> _players = new();

        public Dictionary<ulong, LobbyData> PlayerData => _players;

        public void ResetLobby() => _players = new Dictionary<ulong, LobbyData>();

        public LobbyData? GetData(ulong clientId)
        {
            if (_players.TryGetValue(clientId, out var data))
            {
                return data;
            }

            return null;
        }

        public bool CanStartGame()
        {
            if (_players.Count <= 1)
            {
                return false;
            }

            bool everyoneIsReady = true;

            foreach (var data in _players)
            {
                if (!data.Value.IsReady)
                {
                    everyoneIsReady = false;
                }
            }

            return everyoneIsReady;
        }

        public void AddPoints(ulong clientId, int amount)
        {
            if (_players.TryGetValue(clientId, out var data))
            {
                _players[clientId] = new LobbyData
                {
                    ClientId = data.ClientId,
                    IsReady = true,
                    NickName = data.NickName,
                    Points = data.Points + amount,
                    SkinIndex = data.SkinIndex,
                    BombSkinIndex = data.BombSkinIndex
                };
            }
        }

        public int? GetPlace(ulong clientId)
        {
            int place = 0;

            foreach (var id in _players.Keys)
            {
                if (id == clientId)
                {
                    return place;
                }

                place++;
            }

            Debug.LogError("Lobby: Player not found");

            return null;
        }


        public void SortDescending()
        {
            var sorted = _players.OrderByDescending(data => data.Value.Points);
            _players = sorted.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public void PlayerDisconnected(ulong clientId)
        {
            if (!NetworkManager.Singleton) return;
            if (!NetworkManager.Singleton.IsServer) return;
            _players?.Remove(clientId);
            OnPlayerDisconnected?.Invoke(clientId);
        }

        public void CreatePlayerData(NetworkString nick, ulong clientId, int skinIndex, int bombSkinIndex)
        {
            var data = new LobbyData(nick, clientId, false, 0, skinIndex, bombSkinIndex);

            if (_players.TryAdd(clientId, data))
            {
                OnPlayerConnected?.Invoke(clientId);
            }
            else
            {
                Debug.LogError("Can't create lobby data for the player");
            }

            Debug.Log($"PLAYER DATA: {data.SkinIndex}");
        }

        public void ChangeReadyState(ulong clientId)
        {
            if (_players.TryGetValue(clientId, out var data))
            {
                _players[clientId] = new LobbyData
                {
                    NickName = data.NickName,
                    ClientId = clientId,
                    IsReady = !data.IsReady,
                    Points = data.Points,
                    SkinIndex = data.SkinIndex,
                    BombSkinIndex = data.BombSkinIndex
                };

                OnReadyStateChanged?.Invoke(clientId, _players[clientId].IsReady);
            }
        }
    }
}