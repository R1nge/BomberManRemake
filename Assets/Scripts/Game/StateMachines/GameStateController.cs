using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game.StateMachines
{
    public class GameStateController : NetworkBehaviour
    {
        public event Action<GameStates> OnStateChanged;
        private GameStates _currentState;
        private int _roundsElapsed;
        private GameSettings _gameSettings;

        [Inject]
        private void Inject(GameSettings gameSettings)
        {
            _gameSettings = gameSettings;
        }

        private void Awake()
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += LoadCompleted;

            OnStateChanged += OnOnStateChanged;
        }

        private void OnOnStateChanged(GameStates newState)
        {
            if (newState == GameStates.Loaded)
            {
                SwitchState(GameStates.PreStart);
            }
            // else if (newState == GameStates.PreStart)
            // {
            //     StartCoroutine(Switch_C());
            // }
            else if (newState == GameStates.Tie || newState == GameStates.Win)
            {
                OnStateChanged -= OnOnStateChanged;
                _roundsElapsed++;
                if (_roundsElapsed < (int)_gameSettings.RoundsAmount)
                {
                    print($"Elapsed: {_roundsElapsed}, Total: {_gameSettings.RoundsAmount}");
                    SwitchState(GameStates.NextRound);
                }
                else
                {
                    SwitchState(GameStates.EndGame);
                }
            }
        }

        // private IEnumerator Switch_C()
        // {
        //     yield return new WaitForSeconds(countdownTime);
        //     SwitchState(GameStates.Start);
        // }

        private void LoadCompleted(string _, LoadSceneMode __, List<ulong> clients, List<ulong> ___)
        {
            if (IsServer)
            {
                if (NetworkManager.Singleton.ConnectedClients.Count == clients.Count)
                {
                    SwitchState(GameStates.Loaded);
                }
            }
        }

        public void SwitchState(GameStates newState)
        {
            print($"SWITCHED TO A NEW STATE: {newState}");
            _currentState = newState;
            OnStateChanged?.Invoke(_currentState);
        }
    }
}