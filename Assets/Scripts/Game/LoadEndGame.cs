using Game.StateMachines;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game
{
    public class LoadEndGame : NetworkBehaviour
    {
        private GameStateController2 _gameStateController2;

        [Inject]
        private void Inject(GameStateController2 gameStateController) => _gameStateController2 = gameStateController;

        private void Awake()
        {
            _gameStateController2.OnStateChanged += StateChanged;
        }

        private void StateChanged(GameStates newState)
        {
            switch (newState)
            {
                case GameStates.EndGame:
                    RoundManagerOnOnLoadEndGame();
                    Debug.Log("LOADENDGAME");
                    break;
            }
        }
        
        private void RoundManagerOnOnLoadEndGame()
        {
            NetworkManager.Singleton.SceneManager.LoadScene("GameEnd", LoadSceneMode.Single);
        }

        public override void OnDestroy() => _gameStateController2.OnStateChanged -= StateChanged;
    }
}