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
        private bool _gameEnded;

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

        //Maybe create a queue for the states?
        //I can't because of 'win' and 'tie' states
        //TODO: fix: nextround doesn't work

        private void OnOnStateChanged(GameStates newState)
        {
            if (_gameEnded) return;
            
            if (newState == GameStates.Loaded)
            {
                SwitchState(GameStates.PreStart);
            }
            else if (newState == GameStates.Tie || newState == GameStates.Win)
            {
                _roundsElapsed++;
                if (_roundsElapsed < (int)_gameSettings.RoundsAmount)
                {
                    print($"Elapsed: {_roundsElapsed}, Total: {_gameSettings.RoundsAmount}");
                    SwitchState(GameStates.NextRound);
                }
                else
                {
                    _gameEnded = true;
                    SwitchState(GameStates.EndGame);
                }
            }
            else if (newState == GameStates.NextRound)
            {
                SwitchState(GameStates.Loaded);
            }
        }

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