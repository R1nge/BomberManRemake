using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game
{
    public class RoundManager : NetworkBehaviour
    {
        public event Action OnCleanUpBeforeNextRound;
        public event Action OnLoadNextRound;
        public event Action OnLoadEndGame;
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
            if (IsServer)
            {
                _roundsElapsed++;
                print($"Elapsed: {_roundsElapsed}, Total: {_gameSettings.RoundsAmount}");

                if (_roundsElapsed < _gameSettings.RoundsAmount)
                {
                    CleanupClientRpc();
                    StartCoroutine(LoadNextRound_C());
                }
                else
                {
                    print("LOAD END GAME");
                    OnLoadEndGame?.Invoke();
                }
            }
        }

        private IEnumerator LoadNextRound_C()
        {
            yield return new WaitForSeconds(2f);
            LoadNextRoundClientRpc();
        }

        [ClientRpc]
        private void LoadNextRoundClientRpc()
        {
            OnLoadNextRound?.Invoke();
            print("LOAD NEXT ROUND");
        }

        [ClientRpc]
        private void CleanupClientRpc()
        {
            OnCleanUpBeforeNextRound?.Invoke();
        }

        public override void OnDestroy()
        {
            _gameStateController.OnRoundEnded -= OnRoundEnded;
        }
    }
}