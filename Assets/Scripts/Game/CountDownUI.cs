using System;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game
{
    public class CountDownUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;
        private GameStateController _gameStateController;

        [Inject]
        private void Inject(GameStateController gameStateController)
        {
            _gameStateController = gameStateController;
        }

        private void Start()
        {
            _gameStateController.OnTimeChanged += UpdateUI;
        }

        private void UpdateUI(float time)
        {
            timerText.text = time.ToString("#");
            if (time == 0)
            {
                timerText.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            _gameStateController.OnTimeChanged -= UpdateUI;
        }
    }
}