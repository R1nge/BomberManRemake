using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Game
{
    public class MapGenerator : NetworkBehaviour
    {
        [SerializeField] private MapPreset preset;
        private MapSettings _mapSettings;

        [Inject]
        private void Inject(MapSettings mapSettings)
        {
            _mapSettings = mapSettings;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            Generate();
        }

        private void Generate()
        {
            SpawnFloor();
            SpawnBorders();
            SpawnWalls();
            SpawnDestructables();
        }

        private void SpawnFloor()
        {
            for (int x = 0; x < _mapSettings.Width; x++)
            {
                for (int z = 0; z < _mapSettings.Length; z++)
                {
                    Spawn(preset.Floor, x * preset.Size, z * preset.Size);
                }
            }
        }

        private void SpawnBorders()
        {
            for (int x = -1; x < _mapSettings.Width + 1; x++)
            {
                for (int z = -1; z < _mapSettings.Length + 1; z++)
                {
                    SpawnLeftBorder(x, z);
                    SpawnRightBorder(x, z);
                    SpawnTopBorder(x, z);
                    SpawnBottomBorder(x, z);
                }
            }
        }

        private void SpawnLeftBorder(int x, int z)
        {
            if (x == _mapSettings.Width && z < _mapSettings.Length + 1)
            {
                Spawn(preset.Border, x * preset.Size, z * preset.Size);
            }
        }

        private void SpawnRightBorder(int x, int z)
        {
            if (x == -1 && z < _mapSettings.Length + 1)
            {
                Spawn(preset.Border, x * preset.Size, z * preset.Size);
            }
        }

        private void SpawnTopBorder(int x, int z)
        {
            if (x < _mapSettings.Width + 1 && z == _mapSettings.Length)
            {
                Spawn(preset.Border, x * preset.Size, z * preset.Size);
            }
        }

        private void SpawnBottomBorder(int x, int z)
        {
            if (x < _mapSettings.Width + 1 && z == -1)
            {
                Spawn(preset.Border, x * preset.Size, z * preset.Size);
            }
        }

        private void SpawnWalls()
        {
            for (int x = 0; x < _mapSettings.Width; x++)
            {
                for (int z = 0; z < _mapSettings.Length; z++)
                {
                    if (x % 2 == 1 && z % 2 == 1)
                    {
                        if (IsCenter(x, z)) continue;

                        Spawn(preset.Wall, x * preset.Size, z * preset.Size);
                    }
                }
            }
        }

        private void SpawnDestructables()
        {
            for (int x = 0; x < _mapSettings.Width; x++)
            {
                for (int z = 0; z < _mapSettings.Length; z++)
                {
                    var spawnObstacle = Mathf.FloorToInt(Random.Range(0, 2)) == 0;

                    if (!spawnObstacle) continue;

                    if (IsOdd(x, z)) continue;

                    if (IsLeftBottomCorner(x, z)) continue;

                    if (IsRightBottomCorner(x, z)) continue;

                    if (IsLeftTopCorner(x, z)) continue;

                    if (IsRightTopCorner(x, z)) continue;

                    if (IsCenter(x, z)) continue;

                    Spawn(preset.Destructable, x * preset.Size, z * preset.Size);
                }
            }
        }

        private bool IsOdd(int x, int z) => x % 2 == 1 && z % 2 == 1;

        private bool IsLeftTopCorner(int x, int z)
        {
            if (x == 0 && z == _mapSettings.Length - 1) return true;
            if (x == 1 && z == _mapSettings.Length - 1) return true;
            if (x == 0 && z == _mapSettings.Length - 2) return true;
            return false;
        }

        private bool IsLeftBottomCorner(int x, int z)
        {
            if (x == 0 && z == 0) return true;
            if (x == 1 && z == 0) return true;
            if (x == 0 && z == 1) return true;
            return false;
        }

        private bool IsRightTopCorner(int x, int z)
        {
            if (x == _mapSettings.Width - 1 && z == _mapSettings.Length - 1) return true;
            if (x == _mapSettings.Width - 2 && z == _mapSettings.Length - 1) return true;
            if (x == _mapSettings.Width - 1 && z == _mapSettings.Length - 2) return true;
            return false;
        }

        private bool IsRightBottomCorner(int x, int z)
        {
            if (x == _mapSettings.Width - 1 && z == 0) return true;
            if (x == _mapSettings.Width - 2 && z == 0) return true;
            if (x == _mapSettings.Width - 1 && z == 1) return true;
            return false;
        }

        private bool IsCenter(int x, int z) => x == _mapSettings.Width / 2 && z == _mapSettings.Length / 2;

        private void Spawn(GameObject go, int x, int z)
        {
            var instance = Instantiate(go, new Vector3(x, 0, z), Quaternion.identity);
            instance.GetComponent<NetworkObject>().Spawn(false);
        }
    }
}