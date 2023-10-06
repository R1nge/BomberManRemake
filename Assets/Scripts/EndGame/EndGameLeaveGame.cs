using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EndGame
{
    public class EndGameLeaveGame : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                StartCoroutine(Shutdown_C());
            }

            NetworkManager.Singleton.OnClientDisconnectCallback += LoadMainMenu;
        }

        private void LoadMainMenu(ulong obj)
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }

        private IEnumerator Shutdown_C()
        {
            yield return new WaitForSeconds(3f);
            NetworkManager.Singleton.Shutdown(true);
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }
}