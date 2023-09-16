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
        private const string MAP = "map";
        private SpawnerManager _spawnerManager;
        private Lobby.Lobby _lobby;

        [Inject]
        private void Inject(SpawnerManager spawnerManager, Lobby.Lobby lobby)
        {
            _spawnerManager = spawnerManager;
            _lobby = lobby;
        }

        private void Awake() => _spawnerManager.OnPlayerDeath += UpdateKillFeedServerRpc;

        [ServerRpc(RequireOwnership = false)]
        private void UpdateKillFeedServerRpc(ulong killedId, ulong killerId, DeathType deathType)
        {
            string killedName = _lobby.GetData(killedId).Value.NickName;
            if (deathType == DeathType.Player)
            {
                string killerName = _lobby.GetData(killerId).Value.NickName;
                UpdateKillFeedClientRpc(killedName, killerName);
            }
            else if (deathType == DeathType.Map)
            {
                UpdateKillFeedClientRpc(killedName, MAP);
            }
        }

        [ClientRpc]
        private void UpdateKillFeedClientRpc(string killedName, string killerName)
        {
            var slotInstance = Instantiate(slot, parent);
            slotInstance.Init(killedName, killerName);
        }

        public override void OnDestroy() => _spawnerManager.OnPlayerDeath -= UpdateKillFeedServerRpc;
    }
}