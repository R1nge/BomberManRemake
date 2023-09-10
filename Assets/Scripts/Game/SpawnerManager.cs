using System;
using System.Collections.Generic;
using Skins;
using Skins.Players;
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
        private PlayerSpawnerFPS _playerSpawnerFPS;
        private PlayerSpawnerTPS _playerSpawnerTPS;
        private RoundManager _roundManager;
        private GameSettings _gameSettings;

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
            OnNextRoundServerRpc();
        }

        private void SceneManagerOnOnLoadEventCompleted(string sceneName, LoadSceneMode _, List<ulong> __,
            List<ulong> ___)
        {
            if (sceneName == "Game")
            {
                SpawnServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnServerRpc(ServerRpcParams rpcParams = default)
        {
            var clientId = rpcParams.Receive.SenderClientId;
            print(_gameSettings.GameMode);
            print($"SPAWN ID: {clientId}");
            switch (_gameSettings.GameMode)
            {
                case GameSettings.GameModes.Fps:
                    FpsMode(clientId);
                    break;
                case GameSettings.GameModes.Tps:
                    TpsMode(clientId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            OnPlayerSpawn?.Invoke(clientId);
        }

        private void FpsMode(ulong clientId)
        {
            _playerSpawnerFPS.SpawnServerRpc(clientId);
        }

        private void TpsMode(ulong clientId)
        {
            _playerSpawnerTPS.SpawnServerRpc(clientId);
        }

        public void Despawn(ulong killedId, ulong killerId)
        {
            print($"{killedId} was killed by {killerId}");
            OnPlayerDeath?.Invoke(killedId, killerId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnNextRoundServerRpc(ServerRpcParams rpcParams = default)
        {
            var clientId = rpcParams.Receive.SenderClientId;
            print("NEXT ROUND");
            switch (_gameSettings.GameMode)
            {
                case GameSettings.GameModes.Fps:
                    _playerSpawnerFPS.OnNextRound(clientId);
                    break;
                case GameSettings.GameModes.Tps:
                    _playerSpawnerTPS.OnNextRound(clientId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            OnPlayerSpawn?.Invoke(clientId);
        }

        public override void OnDestroy()
        {
            _roundManager.OnLoadNextRound -= RoundManagerOnOnLoadNextRound;
            base.OnDestroy();
        }
    }
}