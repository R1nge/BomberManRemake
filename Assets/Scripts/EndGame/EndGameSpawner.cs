using System;
using System.Collections.Generic;
using Skins;
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

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _lobby.SortDescending();
            }

            SpawnServerRpc(_skinManager.SelectedSkinIndex);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnServerRpc(int skinIndex, ServerRpcParams rpcParams = default)
        {
            var skin = _skinManager.GetEndGame(skinIndex);
            var clientId = rpcParams.Receive.SenderClientId;

            var player = _diContainer.InstantiatePrefabForComponent<NetworkObject>
            (
                skin,
                spawnPoints[_lobby.GetPlace(clientId)].position,
                Quaternion.identity,
                null
            );

            player.Spawn(true);
            var endGamePlayer = player.GetComponent<EndGamePlayer>();
            endGamePlayer.UpdateNick(clientId);
            endGamePlayer.UpdateScore(clientId);
        }
    }
}