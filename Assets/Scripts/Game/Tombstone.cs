using Misc;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Tombstone : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI nick;
        private Lobby.Lobby _lobby;

        [Inject]
        private void Inject(Lobby.Lobby lobby) => _lobby = lobby;

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            UpdateNickClientRpc(_lobby.GetData(NetworkObject.OwnerClientId).Value.NickName);
        }

        [ClientRpc]
        private void UpdateNickClientRpc(NetworkString nickString) => nick.text = nickString;
    }
}