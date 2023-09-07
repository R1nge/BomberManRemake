using System;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Game
{
    public class KillFeed : NetworkBehaviour
    {
        [SerializeField] private KillFeedSlot slot;
        [SerializeField] private Transform parent;
        private PlayerSpawnerFPS _playerSpawnerFPS;
        private Lobby.Lobby _lobby;

        [Inject]
        private void Inject(PlayerSpawnerFPS playerSpawnerFPS, Lobby.Lobby lobby)
        {
            _playerSpawnerFPS = playerSpawnerFPS;
            _lobby = lobby;
        }

        private void Awake() => _playerSpawnerFPS.OnPlayerDeath += UpdateKillFeedServerRpc;

        [ServerRpc(RequireOwnership = false)]
        private void UpdateKillFeedServerRpc(ulong killedId, ulong killerId)
        {
            string killedName = _lobby.GetData(killedId).Value.NickName;
            string killerName = _lobby.GetData(killerId).Value.NickName;
            UpdateKillFeedClientRpc(killedName, killerName);
        }

        [ClientRpc]
        private void UpdateKillFeedClientRpc(string killedName, string killerName)
        {
            var slotInstance = Instantiate(slot, parent);
            slotInstance.Init(killedName, killerName);
        }

        public override void OnDestroy() => _playerSpawnerFPS.OnPlayerDeath -= UpdateKillFeedServerRpc;
    }
}