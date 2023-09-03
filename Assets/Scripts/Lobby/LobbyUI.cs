using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Lobby
{
    public class LobbyUI : NetworkBehaviour
    {
        [SerializeField] private Button start, ready;
        private Lobby _lobby;

        [Inject]
        private void Inject(Lobby lobby) => _lobby = lobby;

        private void Awake()
        {
            start.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
            });

            ready.onClick.AddListener(() =>
            {
                _lobby.ChangeReadyStateServerRpc(NetworkManager.Singleton.LocalClientId);
                print(NetworkManager.Singleton.LocalClientId);
            });
        }

        private void Start() => start.gameObject.SetActive(IsOwner);
    }
}