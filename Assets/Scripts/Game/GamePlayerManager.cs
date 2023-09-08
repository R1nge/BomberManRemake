using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Game
{
    public class GamePlayerManager : NetworkBehaviour
    {
        private int _playersAlive;
        private readonly List<ulong> _alivePlayers = new(4);
        private SpawnerManager _spawnerManager;
        private GameStateController _gameStateController;

        [Inject]
        private void Inject(SpawnerManager spawnerManager, GameStateController gameStateController)
        {
            _spawnerManager = spawnerManager;
            _gameStateController = gameStateController;
        }

        private void Awake()
        {
            _spawnerManager.OnPlayerSpawn += IncreaseServerRpc;
            _spawnerManager.OnPlayerDeath += DecreaseServerRpc;
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
        private void IncreaseServerRpc(ulong clientId)
        {
            _alivePlayers.Add(clientId);
            _playersAlive++;
            print($"Players alive: {_playersAlive}");
        }

        [ServerRpc(RequireOwnership = false)]
        private void DecreaseServerRpc(ulong killedId, ulong killerId)
        {
            _playersAlive--;
            _alivePlayers.Remove(killedId);
            print($"REMOVED: Players alive: {_playersAlive}");
            if (_playersAlive <= 1)
            {
                _gameStateController.EndGame();
            }
        }

        public override void OnDestroy()
        {
            _spawnerManager.OnPlayerSpawn -= IncreaseServerRpc;
            _spawnerManager.OnPlayerDeath -= DecreaseServerRpc;
            _gameStateController.OnRoundEnded -= Reset;
        }
    }
}