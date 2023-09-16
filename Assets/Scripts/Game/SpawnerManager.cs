using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game
{
    public class SpawnerManager : NetworkBehaviour
    {
        public event Action<ulong> OnPlayerSpawn;
        public event Action<ulong, ulong> OnPlayerDeath;
        private readonly NetworkList<ulong> _playersAlive = new ();
        private PlayerSpawnerFPS _playerSpawnerFPS;
        private PlayerSpawnerTPS _playerSpawnerTPS;
        private GameStateController _gameStateController;
        private GameSettings _gameSettings;
        private Lobby.Lobby _lobby;

        [Inject]
        private void Inject(
            GameStateController gameStateController,
            Lobby.Lobby lobby,
            GameSettings gameSettings,
            PlayerSpawnerFPS playerSpawnerFPS,
            PlayerSpawnerTPS playerSpawnerTPS
        )
        {
            _gameStateController = gameStateController;
            _lobby = lobby;
            _gameSettings = gameSettings;
            _playerSpawnerFPS = playerSpawnerFPS;
            _playerSpawnerTPS = playerSpawnerTPS;
        }

        public ulong LastPlayerAlive => _playersAlive[0];

        public int PlayersAliveCount => _playersAlive.Count;

        private void Awake()
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnOnLoadEventCompleted;
            _gameStateController.OnLoadNextRound += RoundManagerOnOnLoadNextRound;
            _playersAlive.OnListChanged += OnAlivePlayersChanged;
        }

        private void OnAlivePlayersChanged(NetworkListEvent<ulong> changeevent)
        {
            if (_playersAlive.Count <= 1 && changeevent.Type == NetworkListEvent<ulong>.EventType.Remove)
            {
                _gameStateController.Win();
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
            for (int index = 0; index < _lobby.PlayerData.Count; index++)
            {
                var clientId = _lobby.PlayerData[index].ClientId;
                switch (_gameSettings.PerspectiveMode)
                {
                    case GameSettings.PerspectiveModes.Fps:
                        _playerSpawnerFPS.Spawn(clientId, index);
                        break;
                    case GameSettings.PerspectiveModes.Tps:
                        _playerSpawnerTPS.Spawn(clientId, index);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                _playersAlive.Add(clientId);

                OnPlayerSpawn?.Invoke(clientId);
            }
        }

        public void Despawn(ulong killedId, ulong killerId)
        {
            if (IsServer)
            {
                _playersAlive.Remove(killedId);
                OnPlayerDeath?.Invoke(killedId, killerId);
            }
            else
            {
                DespawnServerRpc(killedId, killerId);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void DespawnServerRpc(ulong killedId, ulong killerId)
        {
            _playersAlive.Remove(killedId);
            Debug.LogError($"{killedId} was killed by {killerId}");
            OnPlayerDeath?.Invoke(killedId, killerId);
        }

        public override void OnDestroy()
        {
            _gameStateController.OnLoadNextRound -= RoundManagerOnOnLoadNextRound;
            base.OnDestroy();
        }
    }
}