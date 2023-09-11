using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game
{
    public class SpawnerManager : NetworkBehaviour
    {
        public event Action<ulong> OnPlayerSpawn;
        public event Action<ulong, ulong> OnPlayerDeath;
        private PlayerSpawnerFPS _playerSpawnerFPS;
        private PlayerSpawnerTPS _playerSpawnerTPS;
        private RoundManager _roundManager;
        private GameSettings _gameSettings;
        private Lobby.Lobby _lobby;

        [Inject]
        private void Inject(
            RoundManager roundManager,
            Lobby.Lobby lobby,
            GameSettings gameSettings,
            PlayerSpawnerFPS playerSpawnerFPS,
            PlayerSpawnerTPS playerSpawnerTPS
        )
        {
            _roundManager = roundManager;
            _lobby = lobby;
            _gameSettings = gameSettings;
            _playerSpawnerFPS = playerSpawnerFPS;
            _playerSpawnerTPS = playerSpawnerTPS;
        }

        private void Awake()
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnOnLoadEventCompleted;
            _roundManager.OnLoadNextRound += RoundManagerOnOnLoadNextRound;
        }

        private void RoundManagerOnOnLoadNextRound()
        {
            if (!IsServer) return;
            OnNextRound();
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
                switch (_gameSettings.GameMode)
                {
                    case GameSettings.GameModes.Fps:
                        FpsMode(clientId, index);
                        break;
                    case GameSettings.GameModes.Tps:
                        TpsMode(clientId, index);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                OnPlayerSpawn?.Invoke(clientId);
            }
        }

        private void FpsMode(ulong clientId, int index)
        {
            _playerSpawnerFPS.InitPositions(clientId, index);
        }

        private void TpsMode(ulong clientId, int index)
        {
            _playerSpawnerTPS.InitPositions(clientId, index);
        }

        public void Despawn(ulong killedId, ulong killerId)
        {
            print($"{killedId} was killed by {killerId}");
            OnPlayerDeath?.Invoke(killedId, killerId);
        }

        private void OnNextRound()
        {
            for (int index = 0; index < _lobby.PlayerData.Count; index++)
            {
                var clientId = _lobby.PlayerData[index].ClientId;
                switch (_gameSettings.GameMode)
                {
                    case GameSettings.GameModes.Fps:
                        _playerSpawnerFPS.OnNextRound(clientId, index);
                        break;
                    case GameSettings.GameModes.Tps:
                        _playerSpawnerTPS.OnNextRound(clientId, index);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                OnPlayerSpawn?.Invoke(clientId);
            }
        }

        public override void OnDestroy()
        {
            _roundManager.OnLoadNextRound -= RoundManagerOnOnLoadNextRound;
            base.OnDestroy();
        }
    }
}