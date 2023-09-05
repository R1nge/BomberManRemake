using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace Misc
{
    public class LoadGame : NetworkBehaviour
    {
        private void Awake() => NetworkManager.Singleton.SceneManager.OnUnloadEventCompleted += LoadScene;

        private void LoadScene(string sceneName, LoadSceneMode _, List<ulong> __, List<ulong> ___)
        {
            if (!IsServer) return;
            if (sceneName == "Lobby")
            {
                NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Additive);
            }
        }
    }
}