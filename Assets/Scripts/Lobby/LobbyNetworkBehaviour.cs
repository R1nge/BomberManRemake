using Misc;
using Skins.Bombs;
using Skins.Players;
using Unity.Netcode;
using Zenject;

namespace Lobby
{
    public class LobbyNetworkBehaviour : NetworkBehaviour
    {
        private Lobby _lobby;
        private SkinManager _skinManager;
        private BombSkinManager _bombSkinManager;
        private PlayFabManager _playFabManager;

        [Inject]
        private void Inject(Lobby lobby, SkinManager skinManager, BombSkinManager bombSkinManager,
            PlayFabManager playFabManager)
        {
            _lobby = lobby;
            _skinManager = skinManager;
            _bombSkinManager = bombSkinManager;
            _playFabManager = playFabManager;
        }

        private void Awake()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += PlayerConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += PlayerDisconnected;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                PlayerConnected(0);
            }
        }

        private void PlayerConnected(ulong clientId)
        {
            if (NetworkManager.Singleton.LocalClientId == clientId)
            {
                CreatePlayerData(clientId);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void ChangeReadyStateServerRpc(ulong clientId)
        {
            _lobby.ChangeReadyState(clientId);
        }

        private void CreatePlayerData(ulong clientId)
        {
            CreatePlayerDataServerRpc(clientId, _playFabManager.GetUserName, _skinManager.SelectedSkinIndex,
                _bombSkinManager.SkinIndex);
        }

        [ServerRpc(RequireOwnership = false)]
        private void CreatePlayerDataServerRpc(ulong clientId, NetworkString nick, int skinIndex, int bombSkinIndex)
        {
            _lobby.CreatePlayerData(clientId, nick, skinIndex, bombSkinIndex);
        }

        private void PlayerDisconnected(ulong clientId)
        {
            _lobby.PlayerDisconnected(clientId);
        }

        public override void OnDestroy()
        {
            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= PlayerConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= PlayerDisconnected;
            }

            base.OnDestroy();
        }
    }
}