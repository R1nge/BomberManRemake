using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Lobby
{
    public class PlayerModel : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI ready;

        [ServerRpc(RequireOwnership = false)]
        public void UpdateUIServerRpc(bool isReady)
        {
            UpdateUIClientRpc(isReady);
        }

        [ClientRpc]
        private void UpdateUIClientRpc(bool isReady)
        {
            ready.text = isReady.ToString();
            print("UPDATE READY UI");
        }
    }
}