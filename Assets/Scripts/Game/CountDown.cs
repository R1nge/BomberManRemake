using System;
using Game.StateMachines;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Game
{
    public class CountDown : NetworkBehaviour
    {
        public event Action<float> TimeChanged;
        [SerializeField] private int countdownTime;
        private NetworkVariable<float> _currentTime;
        private GameStateController _gameStateController;
        private bool _timerStarted;

        [Inject]
        private void Inject(GameStateController gameStateController)
        {
            _gameStateController = gameStateController;
        }

        private void Awake()
        {
            NetworkManager.Singleton.NetworkTickSystem.Tick += Tick;
            _gameStateController.OnStateChanged += StateChanged;
            _currentTime = new NetworkVariable<float>(countdownTime);
            _currentTime.OnValueChanged += OnTimeChanged;
        }

        private void OnTimeChanged(float _, float time)
        {
            TimeChanged?.Invoke(time);
            if (time == 0)
            {
                _timerStarted = false;
                _gameStateController.SwitchState(GameStates.Start);
            }
        }

        private void Tick()
        {
            if (_timerStarted)
            {
                var delta = 1f / NetworkManager.Singleton.NetworkTickSystem.TickRate;
                _currentTime.Value = Mathf.Clamp(_currentTime.Value - delta, 0, countdownTime);
            }
        }

        private void StateChanged(GameStates newState)
        {
            switch (newState)
            {
                case GameStates.Loaded:
                    _timerStarted = true;
                    break;
                case GameStates.NextRound:
                    _timerStarted = false;
                    _currentTime.Value = countdownTime;
                    break;
            }
        }

        public override void OnDestroy()
        {
            _gameStateController.OnStateChanged -= StateChanged;
            base.OnDestroy();
        }
    }
}