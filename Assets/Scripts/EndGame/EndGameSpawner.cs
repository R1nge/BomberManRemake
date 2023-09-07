using System;
using System.Collections.Generic;
using Skins;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace EndGame
{
    public class EndGameSpawner : NetworkBehaviour
    {
        [SerializeField] private Transform[] spawnPoints;
        private Lobby.Lobby _lobby;
        private SkinManager _skinManager;

        [Inject]
        private void Inject(Lobby.Lobby lobby, SkinManager skinManager)
        {
            _lobby = lobby;
            _skinManager = skinManager;
        }

        private void Awake()
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += Spawn;
        }

        private void Spawn(string sceneName, LoadSceneMode _, List<ulong> __, List<ulong> ___)
        {
            if (IsServer)
            {
                _lobby.SortDescending();
            }

            SpawnServerRpc(_skinManager.SkinIndex);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnServerRpc(int skinIndex, ServerRpcParams rpcParams = default)
        {
            print($"SPAWN {_lobby.PlayerData.Count}");
            var skin = _skinManager.GetSkin(skinIndex);
            
            var player = Instantiate
            (
                skin,
                spawnPoints[_lobby.GetPlace(rpcParams.Receive.SenderClientId)].position,
                Quaternion.identity
            );
            
            player.Spawn(true);
        }
    }
}