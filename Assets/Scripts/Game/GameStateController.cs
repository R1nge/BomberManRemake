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
        public event Action OnWin;
        public event Action OnTie;
        public event Action OnCleanUpBeforeEnd;
        public event Action OnRoundStarted;
        public event Action OnRoundEnded;
        public event Action OnLoadNextRound;
        public event Action OnLoadEndGame;
        public event Action<float> OnTimeChanged;
        [SerializeField] private int countdownTime;
        private NetworkVariable<float> _time;
        private NetworkVariable<bool> _gameStarted, _gameEnded;
        private int _roundsElapsed;
        private bool _mapLoaded;
        private GameSettings _gameSettings;

        [Inject]
        private void Inject(GameSettings gameSettings) => _gameSettings = gameSettings;

        private void Awake()
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnOnLoadEventCompleted;
            NetworkManager.Singleton.NetworkTickSystem.Tick += Tick;
            _time = new NetworkVariable<float>(countdownTime);
            _time.OnValueChanged += OnValueChanged;
            _gameStarted = new NetworkVariable<bool>();
            _gameEnded = new NetworkVariable<bool>();
        }

        public bool GameStarted => _gameStarted.Value;
        public bool GameEnded => _gameEnded.Value;

        private void SceneManagerOnOnLoadEventCompleted(string sceneName, LoadSceneMode _, List<ulong> loaded,
            List<ulong> ___)
        {
            if (!IsServer) return;
            if (sceneName != "Game") return;
            if (loaded.Count == NetworkManager.Singleton.ConnectedClients.Count)
            {
                ResetTimer();
                OnMapLoaded();
            }
        }

        private void OnMapLoaded()
        {
            if (IsServer)
            {
                StartCoroutine(MapLoaded_C());
            }
        }

        private IEnumerator MapLoaded_C()
        {
            yield return new WaitForSeconds(2f);
            _mapLoaded = true;
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
            if (!IsSpawned) return;
            if (!_mapLoaded) return;
            if (_gameStarted.Value) return;
            var delta = 1f / NetworkManager.Singleton.NetworkTickSystem.TickRate;
            _time.Value -= delta;
        }

        [ClientRpc]
        private void StartGameClientRpc()
        {
            OnRoundStarted?.Invoke();
            Debug.Log("ROUND STARTED");
        }

        public void Win()
        {
            if (!IsServer) return;

            if (!_gameStarted.Value)
            {
                Debug.LogError("Can't end game, because it didn't start");
                return;
            }

            if (_gameEnded.Value)
            {
                Debug.LogError("Can't end game, because it already ended");
                return;
            }

            _gameEnded.Value = true;

            OnWin?.Invoke();

            StartCoroutine(Wait_C(2f));

            EndGame();
        }

        public void Tie()
        {
            if (!IsServer) return;

            if (!_gameStarted.Value)
            {
                Debug.LogError("Can't end game, because it didn't start");
                return;
            }

            if (_gameEnded.Value)
            {
                Debug.LogError("Can't end game, because it already ended");
                return;
            }

            _gameEnded.Value = true;

            OnTie?.Invoke();

            StartCoroutine(Wait_C(2f));

            EndGame();
        }

        private IEnumerator Wait_C(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
        }

        private void EndGame()
        {
            CleanupClientRpc();
            _roundsElapsed++;
            ResetTimer();
            if (_roundsElapsed < (int)_gameSettings.RoundsAmount)
            {
                print($"Elapsed: {_roundsElapsed}, Total: {_gameSettings.RoundsAmount}");
                LoadNextRoundClientRpc();
            }
            else
            {
                print("LOAD END GAME");
                OnLoadEndGame?.Invoke();

                StartCoroutine(Wait_C(1f));
                EndGameClientRpc();
            }
        }

        [ClientRpc]
        private void CleanupClientRpc()
        {
            OnCleanUpBeforeEnd?.Invoke();
        }

        [ClientRpc]
        private void LoadNextRoundClientRpc()
        {
            OnLoadNextRound?.Invoke();
            print("LOAD NEXT ROUND");
        }

        [ClientRpc]
        private void EndGameClientRpc()
        {
            print("ROUND ENDED");
            OnRoundEnded?.Invoke();
        }

        public override void OnDestroy()
        {
            if (!NetworkManager.Singleton) return;
            NetworkManager.Singleton.NetworkTickSystem.Tick -= Tick;
        }
    }
}