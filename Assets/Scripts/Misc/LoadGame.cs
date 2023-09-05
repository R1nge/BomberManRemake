using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace Misc
{
    public class LoadGame : NetworkBehaviour
    {
        private void Awake()
        {
            NetworkManager.Singleton.SceneManager.OnUnloadEventCompleted += LoadScene;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnOnLoadEventCompleted;
        }

        private void LoadScene(string sceneName, LoadSceneMode _, List<ulong> __, List<ulong> ___)
        {
            if (!IsServer) return;
            if (sceneName == "Lobby")
            {
                NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Additive);
            }
        }
        
        
        private void SceneManagerOnOnLoadEventCompleted(string sceneName, LoadSceneMode _, List<ulong> __, List<ulong> ___)
        {
            if (sceneName == "LobbyDataSingleton")
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("MainMenu"));
                SceneManager.sceneUnloaded += SceneUnloaded;
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