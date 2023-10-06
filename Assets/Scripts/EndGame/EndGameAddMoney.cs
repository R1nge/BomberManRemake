using Misc;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace EndGame
{
    public class EndGameAddMoney : NetworkBehaviour
    {
        private Lobby.Lobby _lobby;
        private Wallet _wallet;

        [Inject]
        private void Inject(Lobby.Lobby lobby, Wallet wallet)
        {
            _lobby = lobby;
            _wallet = wallet;
        }

        private void Start()
        {
            GetPlayerScoreServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void GetPlayerScoreServerRpc(ulong localId)
        {
            
            var data = _lobby.GetData(localId);
            if (!data.HasValue)
            {
                Debug.LogError($"Didn't found data of {localId}");
                return;
            }

            var clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[]
                    {
                        localId
                    }
                }
            };

            AddMoneyClientRpc(data.Value.Points, clientRpcParams);
        }

        [ClientRpc]
        private void AddMoneyClientRpc(int money, ClientRpcParams clientRpcParams)
        {
            _wallet.Earn(money);
            Debug.LogError("EARNED");
        }
    }
}