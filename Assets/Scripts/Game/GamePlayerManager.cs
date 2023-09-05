using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game
{
    public class GamePlayerManager : MonoBehaviour
    {
        private readonly List<ulong> _alivePlayers = new();
        private PlayerSpawner _playerSpawner;
        private GameStateController _gameStateController;
        
        [Inject]
        private void Inject(PlayerSpawner playerSpawner, GameStateController gameStateController)
        {
            _playerSpawner = playerSpawner;
            _gameStateController = gameStateController;
        }

        private void Awake()
        {
            _playerSpawner.OnPlayerSpawn += IncreasePlayers;
            _playerSpawner.OnPlayerDeath += DecreasePlayers;
        }

        private void IncreasePlayers(ulong clientId)
        {
            _alivePlayers.Add(clientId);
        }

        private void DecreasePlayers(ulong killedId, ulong killerId)
        {
            _alivePlayers.Remove(killedId);
            if (_alivePlayers.Count == 1)
            {
                _gameStateController.EndGameServerRpc();
            }
        }

        private void OnDestroy()
        {
            _playerSpawner.OnPlayerSpawn -= IncreasePlayers;
            _playerSpawner.OnPlayerDeath -= DecreasePlayers;
        }
    }
}