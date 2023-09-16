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
            _gameStateController.OnLoadNextRound += Hide;
            _gameStateController.OnRoundStarted += Show;
        }

        private void Show() => roundTime.gameObject.SetActive(true);

        private void Hide() => roundTime.gameObject.SetActive(false);

        private void Start() => _gameTimer.CurrentTimer.OnValueChanged += UpdateUI;

        private void UpdateUI(float _, float time) => roundTime.text = "Time left: " + time.ToString("#");
    }
}