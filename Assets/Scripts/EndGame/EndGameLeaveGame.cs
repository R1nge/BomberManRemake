using System;
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
            else
            {
                StartCoroutine(Leave_C());
            }
        }

        private IEnumerator Leave_C()
        {
            yield return new WaitForSeconds(1f);
            NetworkManager.Singleton.Shutdown();
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }

        private IEnumerator Shutdown_C()
        {
            yield return new WaitForSeconds(3f);
            NetworkManager.Singleton.Shutdown();
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }
}