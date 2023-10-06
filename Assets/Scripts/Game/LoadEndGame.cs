using Unity.Netcode;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game
{
    public class LoadEndGame : NetworkBehaviour
    {
        private GameStateController _gameStateController;

        [Inject]
        private void Inject(GameStateController gameStateController) => _gameStateController = gameStateController;

        private void Awake() => _gameStateController.OnLoadEndGame += RoundManagerOnOnLoadEndGame;


        private void RoundManagerOnOnLoadEndGame()
        {
            NetworkManager.Singleton.SceneManager.LoadScene("GameEnd", LoadSceneMode.Single);
        }

        public override void OnDestroy() => _gameStateController.OnLoadEndGame -= RoundManagerOnOnLoadEndGame;
    }
}