using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Game
{
    public class GameStateControllerView : NetworkBehaviour
    {
        public event Action OnGameStarted;

        private void Start()
        {
            if (IsServer && IsOwner)
            {
                StartCoroutine(Start_C());
            }
        }

        private IEnumerator Start_C()
        {
            yield return new WaitForSeconds(15);
            StartGameServerRpc();
        }

        [ServerRpc]
        private void StartGameServerRpc() => StartGameClientRpc();

        [ClientRpc]
        private void StartGameClientRpc()
        {
            OnGameStarted?.Invoke();
            Debug.Log("GAME STARTED");
        }
    }
}