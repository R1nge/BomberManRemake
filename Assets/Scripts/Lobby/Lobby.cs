using System;
using Unity.Netcode;

namespace Lobby
{
    public class Lobby : NetworkBehaviour
    {
        public event Action<ulong> OnPlayerConnected;
        public event Action OnPlayerDisconnected;
        public event Action<ulong, bool> OnReadyStateChanged;
        private NetworkList<LobbyData> _players;

        private void Awake()
        {
            _players = new NetworkList<LobbyData>(new LobbyData[4]);
            NetworkManager.Singleton.OnClientConnectedCallback += PlayerConnected;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            PlayerConnected(0);
        }

        private void PlayerConnected(ulong clientId)
        {
            CreatePlayerData(clientId);
            OnPlayerConnected?.Invoke(clientId);
        }

        private void CreatePlayerData(ulong clientId)
        {
            if (!IsServer) return;
            var data = new LobbyData(clientId, false);
            _players.Add(data);
            print(data.ClientId);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ChangeReadyStateServerRpc(ulong clientId)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i].ClientId == clientId)
                {
                    _players[i] = new LobbyData
                    {
                        ClientId = clientId,
                        IsReady = !_players[i].IsReady
                    };

                    var isReady = _players[i].IsReady;
                    ChangeReadyStateClientRpc(clientId, isReady);
                    OnReadyStateChanged?.Invoke(clientId, isReady);
                    break;
                }
            }
        }

        [ClientRpc]
        private void ChangeReadyStateClientRpc(ulong clientId, bool ready)
        {
           // OnReadyStateChanged?.Invoke(clientId, ready);
        }
    }
}