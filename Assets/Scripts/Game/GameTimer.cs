using Game.StateMachines;
using Unity.Netcode;
using Zenject;

namespace Game
{
    public class GameTimer : NetworkBehaviour
    {
        private NetworkVariable<float> _currentTime;
        private GameSettings _gameSettings;

        private GameStateController _gameStateController;
        private bool _started;

        [Inject]
        private void Inject(GameSettings gameSettings, GameStateController gameStateController)
        {
            _gameSettings = gameSettings;
            _gameStateController = gameStateController;
        }

        private void Awake()
        {
            _currentTime = new NetworkVariable<float>(_gameSettings.RoundTime);
            _currentTime.OnValueChanged += TimeChanged;
            _gameStateController.OnStateChanged += StateChanged;
        }

        private void StateChanged(GameStates newState)
        {
            switch (newState)
            {
                case GameStates.Start:
                    _started = true;
                    break;
                case GameStates.NextRound:
                    _started = false;
                    ResetTime();
                    break;
            }
        }

        private void ResetTime()
        {
            if (!IsServer) return;
            _currentTime.Value = _gameSettings.RoundTime;
        }

        public NetworkVariable<float> CurrentTimer => _currentTime;

        private void TimeChanged(float _, float time)
        {
            if (!IsServer) return;
            if (time <= 0)
            {
                _gameStateController.SwitchState(GameStates.Tie);
            }
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            NetworkManager.Singleton.NetworkTickSystem.Tick += Tick;
        }

        private void Tick()
        {
            if (!_started) return;
            if (_currentTime.Value <= 0) return;
            _currentTime.Value -= 1f / NetworkManager.Singleton.NetworkTickSystem.TickRate;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _gameStateController.OnStateChanged -= StateChanged;
        }
    }
}