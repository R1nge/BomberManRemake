using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game
{
    public class GameStateController : NetworkBehaviour
    {
        public event Action OnGameStarted;
        public event Action OnGameEnded;
        public event Action<float> OnTimeChanged;
        [SerializeField] private int countdownTime;
        private NetworkVariable<float> _time;
        private NetworkVariable<bool> _gameStarted, _gameEnded;

        private void Awake()
        {
            NetworkManager.Singleton.NetworkTickSystem.Tick += Tick;
            _time = new NetworkVariable<float>(countdownTime);
            _time.OnValueChanged += OnValueChanged;
            _gameStarted = new NetworkVariable<bool>();
            _gameEnded = new NetworkVariable<bool>();
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

        [ServerRpc(RequireOwnership = false)]
        public void EndGameServerRpc()
        {
            if (!_gameStarted.Value || _gameEnded.Value)
            {
                Debug.LogError("Can't end game, because it didn't start yet or already ended");
                return;
            }

            Unload("Game");
            _gameEnded.Value = true;
            EndGameClientRpc();
        }

        [ClientRpc]
        private void EndGameClientRpc()
        {
            print("GAMEOVER");
            //TODO: reload game scene or
            //TODO: destroy everything and call start game


            OnGameEnded?.Invoke();
        }

        private void Unload(string sceneName)
        {
            if (sceneName == "Game")
            {
                NetworkManager.Singleton.SceneManager.UnloadScene(SceneManager.GetSceneByName("Game"));
            }
        }
    }
}