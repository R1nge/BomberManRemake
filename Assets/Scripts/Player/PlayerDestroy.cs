using Game;
using Unity.Netcode;
using Zenject;

namespace Player
{
    public class PlayerDestroy : NetworkBehaviour
    {
        private RoundManager _roundManager;

        [Inject]
        private void Inject(RoundManager roundManager) => _roundManager = roundManager;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _roundManager.OnLoadNextRound += Destroy;
            }
        }

        private void Destroy() => NetworkObject.Despawn(true);

        public override void OnDestroy()
        {
            if (IsServer)
            {
                _roundManager.OnLoadNextRound -= Destroy;
            }
        }
    }
}