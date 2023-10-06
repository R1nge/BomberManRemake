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
        private GameStateController _gameStateController;

        [Inject]
        private void Inject(GameStateController gameStateController) => _gameStateController = gameStateController;

        private void Awake()
        {
            _gameTimer = GetComponent<GameTimer>();
            _gameStateController.OnStateChanged += GameStateChanged;
        }

        private void GameStateChanged(GameStates newState)
        {
            switch (newState)
            {
                case GameStates.PreStart:
                    Hide();
                    break;
                case GameStates.Start:
                    Show();
                    break;
            }
        }

        private void Show() => roundTime.gameObject.SetActive(true);

        private void Hide() => roundTime.gameObject.SetActive(false);

        private void Start() => _gameTimer.CurrentTimer.OnValueChanged += UpdateUI;

        private void UpdateUI(float _, float time) => roundTime.text = "Time left: " + time.ToString("#");

        private void OnDestroy() => _gameStateController.OnStateChanged -= GameStateChanged;
    }
}