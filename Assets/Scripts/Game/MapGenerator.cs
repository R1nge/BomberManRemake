using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Random = UnityEngine.Random;

namespace Game
{
    public class MapGenerator : NetworkBehaviour
    {
        [SerializeField] private Transform dynamicParent;
        [SerializeField] private MapPreset[] presets;
        private MapPreset _selected;
        private GameSettings _gameSettings;
        private DiContainer _diContainer;
        private GameStateController _gameStateController;

        [Inject]
        private void Inject(DiContainer diContainer, GameSettings gameSettings, GameStateController gameStateController)
        {
            _diContainer = diContainer;
            _gameSettings = gameSettings;
            _gameStateController = gameStateController;
        }

        private void Awake()
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnOnLoadEventCompleted;
            _gameStateController.OnLoadNextRound += Generate;
        }

        private void SceneManagerOnOnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode,
            List<ulong> clientscompleted, List<ulong> clientstimedout)
        {
            if (scenename == "Game")
            {
                Generate();
            }
        }

        private void Generate()
        {
            if (!IsServer) return;
            print("GENERATE");
            _selected = presets[Random.Range(0, presets.Length)];
            SpawnFloor();
            SpawnBorders();
            SpawnWalls();
            SpawnDestructables();
        }

        private void SpawnFloor()
        {
            for (int x = 0; x < _gameSettings.MapWidth; x++)
            {
                for (int z = 0; z < _gameSettings.MapLength; z++)
                {
                    Spawn(_selected.Floor, x * _selected.Size, z * _selected.Size, Quaternion.identity);
                }
            }
        }

        private void SpawnBorders()
        {
            for (int x = -1; x < _gameSettings.MapWidth + 1; x++)
            {
                for (int z = -1; z < _gameSettings.MapLength + 1; z++)
                {
                    Quaternion cornerRotation;
                    
                    if (LeftBottomCorner(x, z))
                    {
                        cornerRotation = Quaternion.Euler(_selected.LeftBottomCornerRotation);
                        Spawn(_selected.Corner, x * _selected.Size, z * _selected.Size, cornerRotation);
                        continue;
                    }
                    
                    if (LeftTopCorner(x, z))
                    {
                        cornerRotation = Quaternion.Euler(_selected.LeftTopCornerRotation);
                        Spawn(_selected.Corner, x * _selected.Size, z * _selected.Size, cornerRotation);
                        continue;
                    }
                    
                    if (RightBottomCorner(x, z))
                    {
                        cornerRotation = Quaternion.Euler(-_selected.LeftBottomCornerRotation);
                        Spawn(_selected.Corner, x * _selected.Size, z * _selected.Size, cornerRotation);
                        continue;
                    }
                    
                    if (RightTopCorner(x, z))
                    {
                        cornerRotation = Quaternion.Euler(-_selected.LeftTopCornerRotation);
                        Spawn(_selected.Corner, x * _selected.Size, z * _selected.Size, cornerRotation);
                        continue;
                    }

                    SpawnLeftBorder(x, z);
                    SpawnRightBorder(x, z);
                    SpawnTopBorder(x, z);
                    SpawnBottomBorder(x, z);
                }
            }
        }

        private bool LeftBottomCorner(int x, int z)
        {
            return x == -1 && z == -1;
        }

        private bool LeftTopCorner(int x, int z)
        {
            return x == -1 && z == _gameSettings.MapLength;
        }

        private bool RightBottomCorner(int x, int z)
        {
            return x == _gameSettings.MapWidth && z == -1;
        }

        private bool RightTopCorner(int x, int z)
        {
            return x == _gameSettings.MapWidth && z == _gameSettings.MapLength;
        }

        private void SpawnLeftBorder(int x, int z)
        {
            if (x == _gameSettings.MapWidth && z < _gameSettings.MapLength + 1)
            {
                Spawn(_selected.Border, x * _selected.Size, z * _selected.Size, Quaternion.Euler(_selected.LeftBorderRotation));
            }
        }

        private void SpawnRightBorder(int x, int z)
        {
            if (x == -1 && z < _gameSettings.MapLength + 1)
            {
                Spawn(_selected.Border, x * _selected.Size, z * _selected.Size, Quaternion.Euler(-_selected.LeftBorderRotation));
            }
        }

        private void SpawnTopBorder(int x, int z)
        {
            if (x < _gameSettings.MapWidth + 1 && z == _gameSettings.MapLength)
            {
                Spawn(_selected.Border, x * _selected.Size, z * _selected.Size, Quaternion.Euler(_selected.TopBorderRotation));
            }
        }

        private void SpawnBottomBorder(int x, int z)
        {
            if (x < _gameSettings.MapWidth + 1 && z == -1)
            {
                Spawn(_selected.Border, x * _selected.Size, z * _selected.Size, Quaternion.Euler(_selected.BottomBorderRotation));
            }
        }

        private void SpawnWalls()
        {
            for (int x = 0; x < _gameSettings.MapWidth; x++)
            {
                for (int z = 0; z < _gameSettings.MapLength; z++)
                {
                    if (x % 2 == 1 && z % 2 == 1)
                    {
                        if (IsCenter(x, z)) continue;

                        Spawn(_selected.Wall, x * _selected.Size, z * _selected.Size, Quaternion.identity);
                    }
                }
            }
        }

        private void SpawnDestructables()
        {
            for (int x = 0; x < _gameSettings.MapWidth; x++)
            {
                for (int z = 0; z < _gameSettings.MapLength; z++)
                {
                    var spawnObstacle = Mathf.FloorToInt(Random.Range(0, 2)) == 0;

                    if (!spawnObstacle) continue;

                    if (IsOdd(x, z)) continue;

                    if (IsLeftBottomCorner(x, z)) continue;

                    if (IsRightBottomCorner(x, z)) continue;

                    if (IsLeftTopCorner(x, z)) continue;

                    if (IsRightTopCorner(x, z)) continue;

                    if (IsCenter(x, z)) continue;

                    Spawn(_selected.Destructable, x * _selected.Size, z * _selected.Size, Quaternion.identity);
                }
            }
        }

        private bool IsOdd(int x, int z) => x % 2 == 1 && z % 2 == 1;

        private bool IsLeftTopCorner(int x, int z)
        {
            if (x == 0 && z == _gameSettings.MapLength - 1) return true;
            if (x == 1 && z == _gameSettings.MapLength - 1) return true;
            if (x == 0 && z == _gameSettings.MapLength - 2) return true;
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
            if (x == _gameSettings.MapWidth - 1 && z == _gameSettings.MapLength - 1) return true;
            if (x == _gameSettings.MapWidth - 2 && z == _gameSettings.MapLength - 1) return true;
            if (x == _gameSettings.MapWidth - 1 && z == _gameSettings.MapLength - 2) return true;
            return false;
        }

        private bool IsRightBottomCorner(int x, int z)
        {
            if (x == _gameSettings.MapWidth - 1 && z == 0) return true;
            if (x == _gameSettings.MapWidth - 2 && z == 0) return true;
            if (x == _gameSettings.MapWidth - 1 && z == 1) return true;
            return false;
        }

        private bool IsCenter(int x, int z) => x == _gameSettings.MapWidth / 2 && z == _gameSettings.MapLength / 2;

        private void Spawn(GameObject go, int x, int z, Quaternion rotation)
        {
            var instance = _diContainer.InstantiatePrefab(go, new Vector3(x, 0, z), rotation, dynamicParent);
            instance.GetComponent<NetworkObject>().Spawn(true);
            instance.transform.parent = dynamicParent;
        }
    }
}