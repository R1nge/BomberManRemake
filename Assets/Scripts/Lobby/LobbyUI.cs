using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Lobby
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] private Button ready;
        private Lobby _lobby;

        [Inject]
        private void Inject(Lobby lobby)
        {
            _lobby = lobby;
        }

        private void Awake()
        {
            ready.onClick.AddListener(() =>
            {
                _lobby.ChangeReadyStateServerRpc(NetworkManager.Singleton.LocalClientId);
                print(NetworkManager.Singleton.LocalClientId);
            });
        }
    }
}