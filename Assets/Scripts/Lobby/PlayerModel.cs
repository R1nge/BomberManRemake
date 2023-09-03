using TMPro;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Lobby
{
    public class PlayerModel : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI ready;
        private Lobby _lobby;

        [Inject]
        private void Inject(Lobby lobby)
        {
            _lobby = lobby;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _lobby.OnReadyStateChanged += UpdateUIServerRpc;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void UpdateUIServerRpc(ulong clientId, bool isReady)
        {
            ready.text = isReady.ToString();
            print("SERVER UPDATE READY UI");
            UpdateUIClientRpc(clientId, isReady);
        }

        [ClientRpc]
        private void UpdateUIClientRpc(ulong clientId, bool isReady)
        {
            if(IsServer) return;
            ready.text = isReady.ToString();
            print("UPDATE READY UI");
        }
    }
}