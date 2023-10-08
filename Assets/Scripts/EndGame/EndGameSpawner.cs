using System.Collections.Generic;
using Skins.Players;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace EndGame
{
    public class EndGameSpawner : NetworkBehaviour
    {
        [SerializeField] private Transform[] spawnPoints;
        private DiContainer _diContainer;
        private Lobby.Lobby _lobby;
        private SkinManager _skinManager;

        [Inject]
        private void Inject(DiContainer diContainer, Lobby.Lobby lobby, SkinManager skinManager)
        {
            _diContainer = diContainer;
            _lobby = lobby;
            _skinManager = skinManager;
        }

        private void Awake()
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnOnLoadEventCompleted;
        }

        private void SceneManagerOnOnLoadEventCompleted(string _, LoadSceneMode __, List<ulong> completed,
            List<ulong> ___)
        {
            if (IsServer)
            {
                if (_lobby.PlayerData.Count == completed.Count)
                {
                    _lobby.SortDescending();
                    SpawnPlayers();
                }
            }
        }

        private void SpawnPlayers()
        {
            foreach (var data in _lobby.PlayerData.Values)
            {
                var skinIndex = data.SkinIndex;
                var skin = _skinManager.GetEndGame(skinIndex);
                var clientId = data.ClientId;
                var place = _lobby.GetPlace(clientId);

                if (!place.HasValue)
                {
                    Debug.LogError($"Place not found {clientId}", this);
                    return;
                }

                var player = _diContainer.InstantiatePrefabForComponent<NetworkObject>
                (
                    skin,
                    spawnPoints[place.Value].position,
                    Quaternion.identity,
                    null
                );

                player.Spawn(true);
                var endGamePlayer = player.GetComponent<EndGamePlayer>();
                endGamePlayer.UpdateNick(clientId);
                endGamePlayer.UpdateScore(clientId);
            }
        }


        public override void OnDestroy()
        {
            if (NetworkManager.Singleton)
            {
                if (NetworkManager.Singleton.SceneManager != null)
                {
                    NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnOnLoadEventCompleted;
                }
            }

            base.OnDestroy();
        }
    }
}