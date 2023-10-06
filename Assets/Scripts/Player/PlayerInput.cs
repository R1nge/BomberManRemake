using Game.StateMachines;
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
                _gameStateController.OnStateChanged += StateChanged;
            }
        }

        private void StateChanged(GameStates state)
        {
            switch (state)
            {
                case GameStates.Start:
                    EnableInput();
                    break;
                case GameStates.Win:
                case GameStates.Tie:
                    DisableInput();
                    break;
            }
        }

        private void EnableInput() => _enabled.Value = true;

        private void DisableInput() => _enabled.Value = false;

        public override void OnDestroy()
        {
            if (IsServer)
            {
                _gameStateController.OnStateChanged -= StateChanged;
            }

            base.OnDestroy();
        }
    }
}