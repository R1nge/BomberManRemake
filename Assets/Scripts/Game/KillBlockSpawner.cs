using System;
using System.Collections;
using Game.StateMachines;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Game
{
    public class KillBlockSpawner : NetworkBehaviour
    {
        [SerializeField] private Transform dynamicParent;
        [SerializeField] private KillBlock killBlock;
        private GameSettings _gameSettings;
        private WaitForSeconds _waitForSeconds;
        private Coroutine _coroutine;
        private const int HEIGHT = 4;
        private bool _started;
        private GameTimer _gameTimer;
        private GameStateController2 _gameStateController2;

        [Inject]
        private void Inject(GameStateController2 gameStateController, GameSettings gameSettings)
        {
            _gameStateController2 = gameStateController;
            _gameSettings = gameSettings;
        }

        private void Awake()
        {
            _waitForSeconds = new WaitForSeconds(.05f);
            _gameTimer = FindObjectOfType<GameTimer>();
            _gameStateController2.OnStateChanged += StateChanged;
        }

        private void StateChanged(GameStates newState)
        {
            switch (newState)
            {
                case GameStates.Win:
                case GameStates.Tie:
                    _started = false;
                    if (_coroutine != null)
                    {
                        StopCoroutine(_coroutine);
                    }

                    break;
            }
        }

        private void Start() => _gameTimer.CurrentTimer.OnValueChanged += OnValueChanged;

        private void OnValueChanged(float _, float time)
        {
            if (!IsServer) return;
            if (_started) return;
            if (time > 30) return;
            _started = true;
            _coroutine = StartCoroutine(Spawn());
        }

        //TODO: square loop
        private IEnumerator Spawn()
        {
            for (int z = 0; z < _gameSettings.MapLength; z++)
            {
                for (int x = 0; x < _gameSettings.MapWidth; x++)
                {
                    yield return _waitForSeconds;
                    var position = new Vector3(x * 2, HEIGHT, z * 2);
                    var block = Instantiate(killBlock, position, Quaternion.identity);
                    block.GetComponent<NetworkObject>().Spawn(true);
                    block.transform.parent = dynamicParent;
                }
            }
        }
    }
}