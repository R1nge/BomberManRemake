using System.Collections.Generic;
using Game;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Zenject;

namespace Misc
{
    public class LoadGame : NetworkBehaviour
    {
        private GameSettings _gameSettings;

        [Inject]
        private void Inject(GameSettings gameSettings)
        {
            _gameSettings = gameSettings;
        }

        private void Awake()
        {
            NetworkManager.Singleton.SceneManager.OnUnloadEventCompleted += OnUnloadScene;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadScene;
        }

        private void OnUnloadScene(string sceneName, LoadSceneMode _, List<ulong> __, List<ulong> ___)
        {
            if (!IsServer) return;
            if (sceneName == "Lobby")
            {
                if (_gameSettings.MapMode == GameSettings.MapModes.Procedural)
                {
                    NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Additive);
                }
                else
                {
                    NetworkManager.Singleton.SceneManager.LoadScene("Map1", LoadSceneMode.Additive);
                }
            }
        }

        private void OnLoadScene(string sceneName, LoadSceneMode _, List<ulong> __,
            List<ulong> ___)
        {
            if (sceneName == "LobbyDataSingleton")
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("MainMenu"));
                SceneManager.sceneUnloaded += SceneUnloaded;
            }
            else if (sceneName == "Game")
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName("Game"));
            }
            else if (sceneName == "Map1")
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName("Map1"));
            }
        }

        private void SceneUnloaded(Scene scene)
        {
            if (scene.name == "MainMenu")
            {
                NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Additive);
                SceneManager.sceneUnloaded -= SceneUnloaded;
            }
        }
    }
}