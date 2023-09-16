using System;
using System.Collections;
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
        private GameStateController _gameStateController;
        private WaitForSeconds _waitForSeconds;
        private Coroutine _coroutine;
        private const int HEIGHT = 4;
        private bool _started;
        private GameTimer _gameTimer;

        [Inject]
        private void Inject(GameStateController gameStateController, GameSettings gameSettings)
        {
            _gameStateController = gameStateController;
            _gameSettings = gameSettings;
        }

        private void Awake()
        {
            _waitForSeconds = new WaitForSeconds(.05f);
            _gameTimer = FindObjectOfType<GameTimer>();
            _gameStateController.OnCleanUpBeforeEnd += () =>
            {
                _started = false;
                StopCoroutine(_coroutine);
            };
        }

        private void Start()
        {
            _gameTimer.CurrentTimer.OnValueChanged += OnValueChanged;
        }

        private void OnValueChanged(float previousvalue, float time)
        {
            if (!IsServer) return;
            if (_started) return;
            if (time > 30) return;
            _started = true;
            _coroutine = StartCoroutine(Spawn());
        }

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