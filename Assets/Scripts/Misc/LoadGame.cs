using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace Misc
{
    public class LoadGame : NetworkBehaviour
    {
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
                NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Additive);
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