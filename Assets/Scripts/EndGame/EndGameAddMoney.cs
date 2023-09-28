using Misc;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace EndGame
{
    public class EndGameAddMoney : MonoBehaviour
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
            var localId = NetworkManager.Singleton.LocalClientId;
            var data = _lobby.GetData(localId);
            if (!data.HasValue)
            {
                Debug.LogError($"Didn't found data of {localId}");
                return;
            }

            _wallet.Earn(data.Value.Points);
            Debug.LogError("EARNED");
        }
    }
}