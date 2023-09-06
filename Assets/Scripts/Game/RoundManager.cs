using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game
{
    public class RoundManager : MonoBehaviour
    {
        public event Action LoadNextRound;
        public event Action LoadEndGame;
        private int _roundsElapsed;
        private GameStateController _gameStateController;
        private GameSettings _gameSettings;

        [Inject]
        private void Inject(GameStateController gameStateController, GameSettings gameSettings)
        {
            _gameStateController = gameStateController;
            _gameSettings = gameSettings;
        }

        private void Awake()
        {
            _gameStateController.OnRoundEnded += OnRoundEnded;
        }

        private void OnRoundEnded()
        {
            if (_roundsElapsed < _gameSettings.RoundsAmount)
            {
                LoadNextRound?.Invoke();
            }
            else
            {
                LoadEndGame?.Invoke();
            }
        }
    }
}