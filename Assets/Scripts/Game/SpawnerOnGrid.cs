using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Game
{
    public class SpawnerOnGrid : MonoBehaviour
    {
        [SerializeField] private MapPreset preset;
        [SerializeField] private GameObject bombPrefab;
        [SerializeField] private GameObject bombVfxPrefab;
        private DiContainer _diContainer;

        [Inject]
        private void Inject(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        public Bomb SpawnBomb(Vector3 position)
        {
            position = GetNearestGridPosition(position);
            var bomb = _diContainer.InstantiatePrefabForComponent<Bomb>(bombPrefab, position, Quaternion.identity, null);
            bomb.GetComponent<NetworkObject>().Spawn(true);
            print($"SPAWN BOMB AT FLOOR: {position}");
            return bomb;
        }

        public void SpawnDestructable(Vector3 position)
        {
            position = GetNearestGridPosition(position);
            var vfx = Instantiate(preset.Destructable, position, Quaternion.identity);
            vfx.GetComponent<NetworkObject>().Spawn(true);
            print("SPAWN DESTRUCTABLE");
        }

        public void SpawnInject(GameObject prefab, Vector3 position)
        {
            position = GetNearestGridPosition(position);
            var go = _diContainer.InstantiatePrefab(prefab, position, Quaternion.identity, null);
            go.GetComponent<NetworkObject>().Spawn(true);
        }

        public void SpawnBombVfx(Vector3 position)
        {
            position = GetNearestGridPosition(position);
            var vfx = Instantiate(bombVfxPrefab, position, Quaternion.identity);
            vfx.GetComponent<NetworkObject>().Spawn(true);
            print("SPAWN BOMB VFX");
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