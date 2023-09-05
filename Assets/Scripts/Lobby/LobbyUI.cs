using System.Collections.Generic;
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
                NetworkManager.Singleton.SceneManager.UnloadScene(SceneManager.GetSceneByName("Lobby"));
            });

            ready.onClick.AddListener(() =>
            {
                _lobby.ChangeReadyStateServerRpc(NetworkManager.Singleton.LocalClientId);
            });

            NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
            _lobby.OnReadyStateChanged += ReadyStateChanged;
        }

        private void ClientConnected(ulong clientId) => ReadyStateChanged(clientId, false);

        private void ReadyStateChanged(ulong clientId, bool isReady)
        {
            bool everyoneIsReady = true;

            for (int i = 0; i < _lobby.PlayerData.Count; i++)
            {
                if (!_lobby.PlayerData[i].IsReady)
                {
                    everyoneIsReady = false;
                }
            }

            start.interactable = everyoneIsReady;
        }

        private void Start()
        {
            start.gameObject.SetActive(IsOwner);
            start.interactable = false;
        }

        public override void OnDestroy() => _lobby.OnReadyStateChanged -= ReadyStateChanged;
    }
}