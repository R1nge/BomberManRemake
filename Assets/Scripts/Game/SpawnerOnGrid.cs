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

        public void SpawnBomb(Vector3 position)
        {
            position = new Vector3(
                RoundToNearestGrid(position.x),
                0,
                RoundToNearestGrid(position.z)
            );
            print($"SPAWN BOMB AT FLOOR: {position}");
            var bomb = _diContainer.InstantiatePrefab(bombPrefab, position, Quaternion.identity, null);
            bomb.GetComponent<NetworkObject>().Spawn(true);
        }

        public void SpawnDestructable()
        {
            print("SPAWN DESTRUCTABLE");
            //Instantiate(config.Destructable)
        }

        public void SpawnBombVfx(Vector3 position)
        {
            position = new Vector3(
                RoundToNearestGrid(position.x),
                0,
                RoundToNearestGrid(position.z)
            );
            print("SPAWN BOMB VFX");
            var vfx = Instantiate(bombVfxPrefab, position, Quaternion.identity);
            vfx.GetComponent<NetworkObject>().Spawn(true);
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