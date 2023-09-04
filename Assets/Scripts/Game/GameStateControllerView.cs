using System;
using Unity.Netcode;
using UnityEngine;

namespace Game
{
    public class GameStateControllerView : NetworkBehaviour
    {
        public event Action OnGameStarted;
        public event Action<float> OnTimeChanged;
        [SerializeField] private int countdownTime;
        private NetworkVariable<float> _time;
        private NetworkVariable<bool> _gameStarted;

        private void Awake()
        {
            NetworkManager.Singleton.NetworkTickSystem.Tick += Tick;
            _time = new NetworkVariable<float>(countdownTime);
            _time.OnValueChanged += OnValueChanged;
            _gameStarted = new NetworkVariable<bool>();
        }

        private void OnValueChanged(float _, float time)
        {
            OnTimeChanged?.Invoke(time);
            if (time == 0)
            {
                if (IsServer)
                {
                    _gameStarted.Value = true;
                    StartGameClientRpc();
                }

                NetworkManager.Singleton.NetworkTickSystem.Tick -= Tick;
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
            OnGameStarted?.Invoke();
            Debug.Log("GAME STARTED");
        }
    }
}