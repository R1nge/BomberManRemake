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
        public event Action OnPreStartRound;
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
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnOnLoadEventCompleted;
            _gameStateController.OnRoundEnded += OnRoundEnded;
        }

        private void SceneManagerOnOnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode,
            List<ulong> clientscompleted, List<ulong> clientstimedout)
        {
            if (!IsServer) return;
            if (scenename != "Game") return;
            if (clientscompleted.Count == NetworkManager.Singleton.ConnectedClients.Count)
            {
                StartCoroutine(Wait_C());
            }
        }

        private IEnumerator Wait_C()
        {
            yield return new WaitForSeconds(2f);
            OnPreStartClientRpc();
            print("PRESTART");
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
                yield return new WaitForSeconds(2f);
                OnPreStartClientRpc();
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
        private void OnPreStartClientRpc()
        {
            OnPreStartRound?.Invoke();
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