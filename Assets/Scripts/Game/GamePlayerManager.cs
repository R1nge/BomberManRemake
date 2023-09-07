using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Game
{
    public class GamePlayerManager : NetworkBehaviour
    {
        private readonly List<ulong> _alivePlayers = new(4);
        private PlayerSpawnerFPS _playerSpawnerFPS;
        private GameStateController _gameStateController;

        [Inject]
        private void Inject(PlayerSpawnerFPS playerSpawnerFPS, GameStateController gameStateController)
        {
            _playerSpawnerFPS = playerSpawnerFPS;
            _gameStateController = gameStateController;
        }

        private void Awake()
        {
            _playerSpawnerFPS.OnPlayerSpawn += IncreasePlayersServerRpc;
            _playerSpawnerFPS.OnPlayerDeath += DecreasePlayersServerRpc;
            _gameStateController.OnRoundEnded += Reset;
        }

        private void Reset()
        {
            if (IsServer)
            {
                _alivePlayers.Clear();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void IncreasePlayersServerRpc(ulong clientId)
        {
            _alivePlayers.Add(clientId);
            print($"Players alive: {_alivePlayers.Count}");
        }

        [ServerRpc(RequireOwnership = false)]
        private void DecreasePlayersServerRpc(ulong killedId, ulong killerId)
        {
            _alivePlayers.Remove(killedId);
            print($"Players alive: {_alivePlayers.Count}");
            if (_alivePlayers.Count <= 1)
            {
                _gameStateController.EndGameServerRpc();
            }
        }

        public override void OnDestroy()
        {
            _playerSpawnerFPS.OnPlayerSpawn -= IncreasePlayersServerRpc;
            _playerSpawnerFPS.OnPlayerDeath -= DecreasePlayersServerRpc;
            _gameStateController.OnRoundEnded -= Reset;
        }
    }
}