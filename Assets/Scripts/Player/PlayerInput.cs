using Game;
using Unity.Netcode;
using Zenject;

namespace Player
{
    public class PlayerInput : NetworkBehaviour
    {
        private NetworkVariable<bool> _enabled;
        private GameStateController _gameStateController;

        public bool InputEnabled => _enabled.Value;

        [Inject]
        private void Inject(GameStateController gameStateController) => _gameStateController = gameStateController;

        private void Awake() => _enabled = new NetworkVariable<bool>();

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _gameStateController.OnRoundStarted += EnableInput;
            }
        }

        private void EnableInput() => _enabled.Value = true;

        public override void OnDestroy()
        {
            if (IsServer)
            {
                _gameStateController.OnRoundStarted -= EnableInput;
            }

            base.OnDestroy();
        }
    }
}