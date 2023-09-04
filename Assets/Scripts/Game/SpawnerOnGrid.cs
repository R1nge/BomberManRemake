using Unity.Netcode;
using UnityEngine;

namespace Game
{
    public class SpawnerOnGrid : MonoBehaviour
    {
        [SerializeField] private MapConfig config;
        [SerializeField] private GameObject bombPrefab;

        public void SpawnBomb(Vector3 position)
        {
            position = new Vector3(
                RoundToNearestGrid(position.x),
                0,
                RoundToNearestGrid(position.z)
            );
            print($"SPAWN BOMB AT FLOOR: {position}");
            var bomb = Instantiate(bombPrefab, position, Quaternion.identity);
            bomb.GetComponent<NetworkObject>().Spawn(true);
        }

        public void SpawnDestructable()
        {
            print("SPAWN DESTRUCTABLE");
            //Instantiate(config.Destructable)
        }

        private float RoundToNearestGrid(float position)
        {
            var gridSize = config.Size;
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