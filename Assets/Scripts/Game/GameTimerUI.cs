using TMPro;
using UnityEngine;

namespace Game
{
    public class GameTimerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI roundTime;
        private GameTimer _gameTimer;

        private void Awake() => _gameTimer = GetComponent<GameTimer>();

        private void Start()
        {
            _gameTimer.CurrentTimer.OnValueChanged += UpdateUI;
        }

        private void UpdateUI(float _, float time)
        {
            roundTime.text = "Time left: " + time.ToString("#");
        }
    }
}