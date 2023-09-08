using System;
using System.Collections.Generic;
using Skins;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game
{
    public class SpawnerManager : MonoBehaviour
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
            _roundManager.OnLoadNextRound += OnNextRound;
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
            switch (_gameSettings.GameMode)
            {
                case GameSettings.GameModes.Fps:
                    FpsMode(skinIndex);
                    break;
                case GameSettings.GameModes.Tps:
                    TpsMode(skinIndex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            OnPlayerSpawn?.Invoke(rpcParams.Receive.SenderClientId);
        }

        private void FpsMode(int skinIndex)
        {
            _playerSpawnerFPS.SpawnServerRpc(skinIndex);
        }

        private void TpsMode(int skinIndex)
        {
        }

        public void Despawn(ulong killedId, ulong killerId)
        {
            print($"{killedId} was killed by {killerId}");
            OnPlayerDeath?.Invoke(killedId, killerId);
        }

        private void OnNextRound()
        {
            print("NEXT ROUND");
            _playerSpawnerFPS.OnNextRound();
        }
    }
}