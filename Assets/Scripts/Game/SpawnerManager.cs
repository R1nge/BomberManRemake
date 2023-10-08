using System;
using System.Collections.Generic;
using Game.StateMachines;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game
{
    public class SpawnerManager : NetworkBehaviour
    {
        public event Action<ulong> OnPlayerSpawn;
        public event Action<ulong, ulong, DeathType> OnPlayerDeath;
        private NetworkList<ulong> _playersAlive;
        private PlayerSpawnerFPS _playerSpawnerFPS;
        private PlayerSpawnerTPS _playerSpawnerTPS;
        private GameStateController _gameStateController;
        private GameSettings _gameSettings;
        private Lobby.Lobby _lobby;

        [Inject]
        private void Inject(
            GameStateController gameStateController,
            Lobby.Lobby lobby,
            GameSettings gameSettings
        )
        {
            _gameStateController = gameStateController;
            _lobby = lobby;
            _gameSettings = gameSettings;
        }

        public ulong LastPlayerAlive => _playersAlive[0];

        public int PlayersAliveCount => _playersAlive.Count;

        private void Awake()
        {
            _playersAlive = new();
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnOnLoadEventCompleted;
            _gameStateController.OnStateChanged += StateChanged;
            _playersAlive.OnListChanged += OnAlivePlayersChanged;

            _playerSpawnerFPS = GetComponent<PlayerSpawnerFPS>();
            _playerSpawnerTPS = GetComponent<PlayerSpawnerTPS>();
        }

        private void StateChanged(GameStates newState)
        {
            switch (newState)
            {
                case GameStates.NextRound:
                    RoundManagerOnOnLoadNextRound();
                    break;
            }
        }

        private void OnAlivePlayersChanged(NetworkListEvent<ulong> listEvent)
        {
            if (_playersAlive.Count <= 1 && listEvent.Type == NetworkListEvent<ulong>.EventType.Remove)
            {
                _gameStateController.SwitchState(GameStates.Win);
            }
        }

        private void RoundManagerOnOnLoadNextRound()
        {
            if (!IsServer) return;
            _playersAlive.Clear();
            Spawn();
        }

        private void SceneManagerOnOnLoadEventCompleted(string sceneName, LoadSceneMode _, List<ulong> loadedClients,
            List<ulong> ___)
        {
            if (!IsServer) return;
            if (sceneName != "Game") return;
            Spawn();
        }

        private void Spawn()
        {
            var position = 0;
            
            foreach (var data in _lobby.PlayerData)
            {
                var clientId = data.Key;
                switch (_gameSettings.PerspectiveMode)
                {
                    case GameSettings.PerspectiveModes.Fps:
                        _playerSpawnerFPS.Spawn(clientId, position);
                        break;
                    case GameSettings.PerspectiveModes.Tps:
                        _playerSpawnerTPS.Spawn(clientId, position);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _playersAlive.Add(clientId);

                OnPlayerSpawn?.Invoke(clientId);

                position++;
            }
        }

        public void Despawn(ulong killedId, ulong killerId, DeathType deathType)
        {
            if (IsServer)
            {
                _playersAlive.Remove(killedId);
                OnPlayerDeath?.Invoke(killedId, killerId, deathType);
            }
            else
            {
                DespawnServerRpc(killedId, killerId, deathType);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void DespawnServerRpc(ulong killedId, ulong killerId, DeathType deathType)
        {
            _playersAlive.Remove(killedId);
            Debug.LogError($"{killedId} was killed by {killerId}");
            OnPlayerDeath?.Invoke(killedId, killerId, deathType);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _gameStateController.OnStateChanged -= StateChanged;
            _playersAlive.OnListChanged -= OnAlivePlayersChanged;
        }
    }
}