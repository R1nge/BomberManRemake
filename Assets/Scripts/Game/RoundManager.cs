using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Game
{
    public class RoundManager : NetworkBehaviour
    {
        public event Action OnCleanUpBeforeEnd;
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

                StartCoroutine(OnRoundEnded_C());
            }
        }

        private IEnumerator OnRoundEnded_C()
        {
            CleanupClientRpc();

            yield return new WaitForSeconds(2f);

            if (_roundsElapsed < _gameSettings.RoundsAmount)
            {
                yield return new WaitForSeconds(2f);
                LoadNextRoundClientRpc();
            }
            else
            {
                print("LOAD END GAME");
                OnLoadEndGame?.Invoke();
            }
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
            OnCleanUpBeforeEnd?.Invoke();
        }

        public override void OnDestroy()
        {
            _gameStateController.OnRoundEnded -= OnRoundEnded;
        }
    }
}