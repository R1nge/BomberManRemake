using System;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game
{
    public class CountDownUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;
        private GameStateControllerView _gameStateControllerView;

        [Inject]
        private void Inject(GameStateControllerView gameStateControllerView)
        {
            _gameStateControllerView = gameStateControllerView;
        }

        private void Start()
        {
            _gameStateControllerView.OnTimeChanged += UpdateUI;
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
            _gameStateControllerView.OnTimeChanged -= UpdateUI;
        }
    }
}