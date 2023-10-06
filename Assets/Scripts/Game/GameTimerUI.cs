using Game.StateMachines;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game
{
    public class GameTimerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI roundTime;
        private GameTimer _gameTimer;
        private GameStateController2 _gameStateController2;

        [Inject]
        private void Inject(GameStateController2 gameStateController2) => _gameStateController2 = gameStateController2;

        private void Awake()
        {
            _gameTimer = GetComponent<GameTimer>();
            _gameStateController2.OnStateChanged += GameStateChanged;
        }

        private void GameStateChanged(GameStates newState)
        {
            switch (newState)
            {
                case GameStates.PreStart:
                    Show();
                    break;
                case GameStates.Start:
                    Hide();
                    break;
            }
        }

        private void Show() => roundTime.gameObject.SetActive(true);

        private void Hide() => roundTime.gameObject.SetActive(false);

        private void Start() => _gameTimer.CurrentTimer.OnValueChanged += UpdateUI;

        private void UpdateUI(float _, float time) => roundTime.text = "Time left: " + time.ToString("#");

        private void OnDestroy() => _gameStateController2.OnStateChanged -= GameStateChanged;
    }
}