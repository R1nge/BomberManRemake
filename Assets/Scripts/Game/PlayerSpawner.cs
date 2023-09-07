using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Random = UnityEngine.Random;

namespace Game
{
    public class PlayerSpawner : NetworkBehaviour
    {
        public event Action<ulong> OnPlayerSpawn;
        public event Action<ulong, ulong> OnPlayerDeath;
        [SerializeField] private Transform dynamicParent;
        [SerializeField] private MapPreset mapPreset;
        [SerializeField] private GameObject playerPrefab;
        private bool _leftTop, _rightTop, _rightBottom, _leftBottom;
        private DiContainer _diContainer;
        private GameSettings _gameSettings;
        private RoundManager _roundManager;

        [Inject]
        private void Inject(DiContainer diContainer, GameSettings gameSettings, RoundManager roundManager)
        {
            _diContainer = diContainer;
            _gameSettings = gameSettings;
            _roundManager = roundManager;
        }

        private void Awake()
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnOnLoadEventCompleted;
            _roundManager.OnLoadNextRound += OnNextRound;
        }

        private void OnNextRound()
        {
            if (IsServer)
            {
                _leftTop = false;
                _rightTop = false;
                _rightBottom = false;
                _leftBottom = false;
            }

            SpawnServerRpc();
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
            var position = PickPosition();
            var player = _diContainer.InstantiatePrefab(playerPrefab, position, Quaternion.identity, null);
            player.transform.parent = dynamicParent;
            player.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId, true);
            player.transform.position = position;
            player.transform.parent = dynamicParent;
            OnPlayerSpawn?.Invoke(rpcParams.Receive.SenderClientId);
        }

        public void Despawn(ulong killedId, ulong killerId)
        {
            print($"{killedId} was killed by {killerId}");
            OnPlayerDeath?.Invoke(killedId, killerId);
        }

        private Vector3 PickPosition()
        {
            var position = Random.Range(0, 4);

            if (position == 0)
            {
                return LeftBottomCorner();
            }

            if (position == 1)
            {
                return LeftTopCorner();
            }

            if (position == 2)
            {
                return RightTopCorner();
            }

            if (position == 3)
            {
                return RightBottomCorner();
            }

            return PickPosition();
        }

        private Vector3 LeftTopCorner()
        {
            if (!_leftTop)
            {
                _leftTop = true;
                print("Left Top");
                return new Vector3(0, 0, (_gameSettings.MapLength - 1) * mapPreset.Size);
            }

            return PickPosition();
        }

        private Vector3 RightTopCorner()
        {
            if (!_rightTop)
            {
                _rightTop = true;
                print("Right Top");
                return new Vector3((_gameSettings.MapWidth - 1) * mapPreset.Size, 0,
                    (_gameSettings.MapLength - 1) * mapPreset.Size);
            }

            return PickPosition();
        }

        private Vector3 RightBottomCorner()
        {
            if (!_rightBottom)
            {
                _rightBottom = true;
                print("Right Bottom");
                return new Vector3((_gameSettings.MapWidth - 1) * mapPreset.Size, 0, 0);
            }

            return PickPosition();
        }

        private Vector3 LeftBottomCorner()
        {
            if (!_leftBottom)
            {
                _leftBottom = true;
                print("Left Bottom");
                return new Vector3(0, 0, 0);
            }

            return PickPosition();
        }

        public override void OnDestroy() => _roundManager.OnLoadNextRound -= OnNextRound;
    }
}