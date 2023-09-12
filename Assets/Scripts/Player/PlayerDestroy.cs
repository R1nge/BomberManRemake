using Game;
using Unity.Netcode;
using Zenject;

namespace Player
{
    public class PlayerDestroy : NetworkBehaviour
    {
        private GameStateController _gameStateController;

        [Inject]
        private void Inject(GameStateController gameStateController) => _gameStateController = gameStateController;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _gameStateController.OnLoadNextRound += Destroy;
            }
        }

        private void Destroy() => NetworkObject.Despawn(true);

        public override void OnDestroy()
        {
            if (IsServer)
            {
                _gameStateController.OnLoadNextRound -= Destroy;
            }
        }
    }
}