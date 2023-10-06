using Game.StateMachines;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Game
{
    public class CountDownUI : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;
        private GameStateController2 _gameStateController;

        [Inject]
        private void Inject(GameStateController2 gameStateController) => _gameStateController = gameStateController;

        // private void Awake() => _gameStateController.OnCountDownTimeChanged += UpdateUI;
        //
        // private void UpdateUI(float time)
        // {
        //     timerText.text = time.ToString("#");
        //     if (time == 0)
        //     {
        //         timerText.text = "";
        //     }
        // }
        //
        // public override void OnDestroy() => _gameStateController.OnCountDownTimeChanged -= UpdateUI;
    }
}