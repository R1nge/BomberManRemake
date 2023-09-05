using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lobby
{
    public class Lobby : NetworkBehaviour
    {
        public event Action<ulong> OnPlayerConnected;
        public event Action OnPlayerDisconnected;
        public event Action<ulong, bool> OnReadyStateChanged;
        private NetworkList<LobbyData> _players;

        public NetworkList<LobbyData> PlayerData => _players;

        public LobbyData? GetData(ulong clientId)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i].ClientId == clientId)
                {
                    return _players[i];
                }
            }

            Debug.LogError("Player data not found", this);

            return null;
        }

        private void Awake()
        {
            _players ??= new NetworkList<LobbyData>();
            NetworkManager.Singleton.OnClientConnectedCallback += PlayerConnected;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnOnLoadEventCompleted;
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

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            PlayerConnected(0);
        }

        private void PlayerConnected(ulong clientId)
        {
            CreatePlayerData(clientId);
            OnPlayerConnected?.Invoke(clientId);
        }

        private void CreatePlayerData(ulong clientId)
        {
            if (!IsServer) return;
            var data = new LobbyData(PlayerPrefs.GetString("Nick"), clientId, false);
            _players.Add(data);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ChangeReadyStateServerRpc(ulong clientId)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i].ClientId == clientId)
                {
                    _players[i] = new LobbyData
                    {
                        NickName = _players[i].NickName,
                        ClientId = clientId,
                        IsReady = !_players[i].IsReady
                    };

                    var isReady = _players[i].IsReady;

                    ChangeReadyStateClientRpc(clientId, isReady);
                    break;
                }
            }
        }

        [ClientRpc]
        private void ChangeReadyStateClientRpc(ulong clientId, bool ready)
        {
            OnReadyStateChanged?.Invoke(clientId, ready);
        }
    }
}