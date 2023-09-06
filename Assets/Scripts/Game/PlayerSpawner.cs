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
        private NetworkVariable<bool> _leftTop, _rightTop, _rightBottom, _leftBottom;
        private DiContainer _diContainer;
        private GameSettings _gameSettings;

        private void Awake()
        {
            _leftTop = new NetworkVariable<bool>();
            _rightTop = new NetworkVariable<bool>();
            _rightBottom = new NetworkVariable<bool>();
            _leftBottom = new NetworkVariable<bool>();
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnOnLoadEventCompleted;
        }

        private void SceneManagerOnOnLoadEventCompleted(string sceneName, LoadSceneMode _, List<ulong> __,
            List<ulong> ___)
        {
            if (!IsServer) return;
            if (sceneName == "Game")
            {
                SpawnServerRpc();
            }
        }

        [Inject]
        private void Inject(DiContainer diContainer, GameSettings gameSettings)
        {
            _diContainer = diContainer;
            _gameSettings = gameSettings;
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
                if (!_leftBottom.Value)
                {
                    return LeftBottomCorner();
                }
            }
            else if (position == 1)
            {
                if (!_leftTop.Value)
                {
                    return LeftTopCorner();
                }
            }
            else if (position == 2)
            {
                if (!_rightTop.Value)
                {
                    return RightTopCorner();
                }
            }
            else if (position == 3)
            {
                if (!_rightBottom.Value)
                {
                    return RightBottomCorner();
                }
            }

            return PickPosition();
        }

        private Vector3 LeftTopCorner()
        {
            _leftTop.Value = true;
            print("Left Top");
            return new Vector3(0, 0, (_gameSettings.MapLength - 1) * mapPreset.Size);
        }

        private Vector3 RightTopCorner()
        {
            _rightTop.Value = true;
            print("Right Top");
            return new Vector3((_gameSettings.MapWidth - 1) * mapPreset.Size, 0,
                (_gameSettings.MapLength - 1) * mapPreset.Size);
        }

        private Vector3 RightBottomCorner()
        {
            _rightBottom.Value = true;
            print("Right Bottom");
            return new Vector3((_gameSettings.MapWidth - 1) * mapPreset.Size, 0, 0);
        }

        private Vector3 LeftBottomCorner()
        {
            _leftBottom.Value = true;
            print("Left Bottom");
            return new Vector3(0, 0, 0);
        }
    }
}