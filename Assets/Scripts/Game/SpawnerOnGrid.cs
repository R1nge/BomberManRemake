using Skins.Bombs;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Game
{
    public class SpawnerOnGrid : MonoBehaviour
    {
        [SerializeField] private Transform dynamicParent;
        [SerializeField] private MapPreset preset;
        [SerializeField] private GameObject bombVfxPrefab;
        private DiContainer _diContainer;
        private BombSkinManager _bombSkinManager;
        private Lobby.Lobby _lobby;
        private NetworkObjectPool _networkObjectPool;

        [Inject]
        private void Inject(DiContainer diContainer, BombSkinManager bombSkinManager, Lobby.Lobby lobby, NetworkObjectPool networkObjectPool)
        {
            _diContainer = diContainer;
            _bombSkinManager = bombSkinManager;
            _lobby = lobby;
            _networkObjectPool = networkObjectPool;
        }

        public Bomb SpawnBomb(Vector3 position, ulong ownerId)
        {
            position = GetNearestGridPosition(position);
            var prefabIndex = _lobby.GetData(ownerId).Value.BombSkinIndex;
            var prefab = _bombSkinManager.Skins[prefabIndex].BombPrefab;
            var bomb = _diContainer.InstantiatePrefabForComponent<Bomb>(prefab, position, Quaternion.identity,
                dynamicParent);
            bomb.transform.parent = dynamicParent;
            bomb.GetComponent<NetworkObject>().SpawnWithOwnership(ownerId, true);
            bomb.transform.parent = dynamicParent;
            return bomb;
        }

        public void SpawnDestructable(Vector3 position)
        {
            position = GetNearestGridPosition(position);
            var destructable = Instantiate(preset.Destructable, position, Quaternion.identity);
            destructable.transform.parent = dynamicParent;
            destructable.GetComponent<NetworkObject>().Spawn(true);
            destructable.transform.parent = dynamicParent;
        }

        public void SpawnInject(GameObject prefab, Vector3 position)
        {
            position = GetNearestGridPosition(position);
            var go = _diContainer.InstantiatePrefab(prefab, position, Quaternion.identity, dynamicParent);
            go.transform.parent = dynamicParent;
            go.GetComponent<NetworkObject>().Spawn(true);
            go.transform.parent = dynamicParent;
        }

        public void SpawnInjectWithOwnership(GameObject prefab, Vector3 position, ulong ownerId, Vector3 offset)
        {
            position = GetNearestGridPosition(position);
            position += offset;
            var go = _diContainer.InstantiatePrefab(prefab, position, Quaternion.identity, dynamicParent);
            go.transform.parent = dynamicParent;
            go.GetComponent<NetworkObject>().SpawnWithOwnership(ownerId, true);
            go.transform.parent = dynamicParent;
        }

        public void SpawnBombVfx(Vector3 position)
        {
            position = GetNearestGridPosition(position);
            _networkObjectPool.GetNetworkObject(bombVfxPrefab.name, position, Quaternion.identity);
        }

        private Vector3 GetNearestGridPosition(Vector3 position)
        {
            position = new Vector3(
                RoundToNearestGrid(position.x),
                0,
                RoundToNearestGrid(position.z)
            );
            return position;
        }

        private float RoundToNearestGrid(float position)
        {
            var gridSize = preset.Size;
            float xDiff = position % gridSize;
            position -= xDiff;
            if (xDiff > gridSize / 2f)
            {
                position += gridSize;
            }

            return position;
        }
    }
}