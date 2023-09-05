using System;
using Unity.Netcode;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Game
{
    public class PlayerSpawner : NetworkBehaviour
    {
        public event Action<ulong> OnPlayerSpawn; 
        public event Action<ulong, ulong> OnPlayerDeath; 
        [SerializeField] private MapPreset mapPreset;
        [SerializeField] private GameObject playerPrefab;
        private NetworkVariable<bool> _leftTop, _rightTop, _rightBottom, _leftBottom;
        private DiContainer _diContainer;
        private MapSettings _mapSettings;

        private void Awake()
        {
            _leftTop = new NetworkVariable<bool>();
            _rightTop = new NetworkVariable<bool>();
            _rightBottom = new NetworkVariable<bool>();
            _leftBottom = new NetworkVariable<bool>();
        }

        [Inject]
        private void Inject(DiContainer diContainer, MapSettings mapSettings)
        {
            _diContainer = diContainer;
            _mapSettings = mapSettings;
        }

        public override void OnNetworkSpawn() => SpawnServerRpc();

        [ServerRpc(RequireOwnership = false)]
        private void SpawnServerRpc(ServerRpcParams rpcParams = default)
        {
            var player = _diContainer.InstantiatePrefab(playerPrefab, PickPosition(), Quaternion.identity, null);
            player.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId);
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
            else
            {
                return Vector3.zero;
            }

            return PickPosition();
        }

        private Vector3 LeftTopCorner()
        {
            _leftTop.Value = true;
            print("Left Top");
            return new Vector3(0, 0, (_mapSettings.Length - 1) * mapPreset.Size);
        }

        private Vector3 RightTopCorner()
        {
            _rightTop.Value = true;
            print("Right Top");
            return new Vector3((_mapSettings.Width - 1) * mapPreset.Size, 0, (_mapSettings.Length - 1) * mapPreset.Size);
        }

        private Vector3 RightBottomCorner()
        {
            _rightBottom.Value = true;
            print("Right Bottom");
            return new Vector3(0, 0, (_mapSettings.Length - 1) * mapPreset.Size);
        }

        private Vector3 LeftBottomCorner()
        {
            _leftBottom.Value = true;
            print("Left Bottom");
            return new Vector3(0, 0, 0);
        }
    }
}