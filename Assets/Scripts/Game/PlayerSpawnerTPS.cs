using Skins;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Game
{
    public class PlayerSpawnerTPS : NetworkBehaviour
    {
        [SerializeField] private Transform dynamicParent;
        [SerializeField] private MapPreset mapPreset;
        private bool _leftTop, _rightTop, _rightBottom, _leftBottom;
        private DiContainer _diContainer;
        private GameSettings _gameSettings;
        private SkinManager _skinManager;

        [Inject]
        private void Inject(DiContainer diContainer, GameSettings gameSettings, SkinManager skinManager)
        {
            _diContainer = diContainer;
            _gameSettings = gameSettings;
            _skinManager = skinManager;
        }

        public void OnNextRound(ulong clientId)
        {
            if (IsServer)
            {
                _leftTop = false;
                _rightTop = false;
                _rightBottom = false;
                _leftBottom = false;
            }

            SpawnServerRpc(clientId, _skinManager.SkinIndex);
        }

        [ServerRpc(RequireOwnership = false)]
        public void SpawnServerRpc(ulong clientId, int skinIndex)
        {
            var position = PickPosition();
            var player = _diContainer.InstantiatePrefab(_skinManager.GetSkinTPS(skinIndex), position,
                Quaternion.identity, null);
            player.transform.parent = dynamicParent;
            player.GetComponent<NetworkObject>().SpawnWithOwnership(clientId, true);
            player.transform.position = position;
            player.transform.parent = dynamicParent;
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
    }
}