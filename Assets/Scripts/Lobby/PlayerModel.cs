using Misc;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Lobby
{
    public class PlayerModel : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI nickname, ready;

        [ServerRpc(RequireOwnership = false)]
        public void UpdateUIServerRpc(NetworkString nickName, bool isReady)
        {
            UpdateUIClientRpc(nickName, isReady);
        }

        [ClientRpc]
        private void UpdateUIClientRpc(NetworkString nickName, bool isReady)
        {
            nickname.text = nickName;
            ready.text = isReady.ToString();
        }
    }
}