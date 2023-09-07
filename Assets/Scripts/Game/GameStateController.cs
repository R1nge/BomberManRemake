using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game
{
    public class GameStateController : NetworkBehaviour
    {
        public event Action OnRoundStarted;
        public event Action OnRoundEnded;
        public event Action<float> OnTimeChanged;
        [SerializeField] private int countdownTime;
        private NetworkVariable<float> _time;
        private NetworkVariable<bool> _gameStarted, _gameEnded;
        private RoundManager _roundManager;

        [Inject]
        private void Inject(RoundManager roundManager)
        {
            _roundManager = roundManager;
        }

        private void Awake()
        {
            NetworkManager.Singleton.NetworkTickSystem.Tick += Tick;
            _time = new NetworkVariable<float>(countdownTime);
            _time.OnValueChanged += OnValueChanged;
            _gameStarted = new NetworkVariable<bool>();
            _gameEnded = new NetworkVariable<bool>();
            _roundManager.OnCleanUpBeforeNextRound += ResetTimer;
        }

        private void ResetTimer()
        {
            if (IsServer)
            {
                _time.Value = countdownTime;
                _gameStarted.Value = false;
                _gameEnded.Value = false;
            }
        }

        private void OnValueChanged(float _, float time)
        {
            OnTimeChanged?.Invoke(time);
            if (time <= 0)
            {
                if (IsServer)
                {
                    _gameStarted.Value = true;
                    StartGameClientRpc();
                }
            }
        }

        private void Tick()
        {
            if (!IsServer) return;
            if (_gameStarted.Value) return;
            var delta = 1f / NetworkManager.Singleton.NetworkTickSystem.TickRate;
            _time.Value -= delta;
        }

        [ClientRpc]
        private void StartGameClientRpc()
        {
            OnRoundStarted?.Invoke();
            Debug.Log("GAME STARTED");
        }

        [ServerRpc(RequireOwnership = false)]
        public void EndGameServerRpc()
        {
            if (!_gameStarted.Value || _gameEnded.Value)
            {
                Debug.LogError("Can't end game, because it didn't start yet or already ended");
                return;
            }

            _gameEnded.Value = true;
            EndGameClientRpc();
        }

        [ClientRpc]
        private void EndGameClientRpc()
        {
            print("GAMEOVER");
            OnRoundEnded?.Invoke();
        }
    }
}