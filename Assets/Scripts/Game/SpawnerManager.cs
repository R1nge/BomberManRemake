using System;
using System.Collections.Generic;
using Skins;
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
        private SkinManager _skinManager;
        private GameSettings _gameSettings;

        [Inject]
        private void Inject(
            RoundManager roundManager,
            SkinManager skinManager,
            GameSettings gameSettings,
            PlayerSpawnerFPS playerSpawnerFPS,
            PlayerSpawnerTPS playerSpawnerTPS
        )
        {
            _roundManager = roundManager;
            _skinManager = skinManager;
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
                SpawnServerRpc(_skinManager.SkinIndex);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnServerRpc(int skinIndex, ServerRpcParams rpcParams = default)
        {
            var clientId = rpcParams.Receive.SenderClientId;
            print(_gameSettings.GameMode);
            print($"SPAWN ID: {clientId}");
            switch (_gameSettings.GameMode)
            {
                case GameSettings.GameModes.Fps:
                    FpsMode(clientId, skinIndex);
                    break;
                case GameSettings.GameModes.Tps:
                    TpsMode(clientId, skinIndex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            OnPlayerSpawn?.Invoke(clientId);
        }

        private void FpsMode(ulong clientId, int skinIndex)
        {
            _playerSpawnerFPS.SpawnServerRpc(clientId, skinIndex);
        }

        private void TpsMode(ulong clientId, int skinIndex)
        {
            _playerSpawnerTPS.SpawnServerRpc(clientId, skinIndex);
        }

        public void Despawn(ulong killedId, ulong killerId)
        {
            print($"{killedId} was killed by {killerId}");
            OnPlayerDeath?.Invoke(killedId, killerId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnNextRoundServerRpc(ServerRpcParams rpcParams = default)
        {
            print("NEXT ROUND");
            switch (_gameSettings.GameMode)
            {
                case GameSettings.GameModes.Fps:
                    _playerSpawnerFPS.OnNextRound(rpcParams.Receive.SenderClientId);
                    break;
                case GameSettings.GameModes.Tps:
                    _playerSpawnerTPS.OnNextRound(rpcParams.Receive.SenderClientId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void OnDestroy()
        {
            _roundManager.OnLoadNextRound -= RoundManagerOnOnLoadNextRound;
            base.OnDestroy();
        }
    }
}