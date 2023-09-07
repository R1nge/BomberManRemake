using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game
{
    public class LoadEndGame : NetworkBehaviour
    {
        private RoundManager _roundManager;

        [Inject]
        private void Inject(RoundManager roundManager)
        {
            _roundManager = roundManager;
        }

        private void Awake()
        {
            _roundManager.OnLoadEndGame += RoundManagerOnOnLoadEndGame;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
        }

        private void OnSceneLoaded(string sceneName, LoadSceneMode _, List<ulong> __, List<ulong> ___)
        {
            if (sceneName == "GameEnd")
            {
                if (IsServer)
                {
                    NetworkManager.Singleton.SceneManager.UnloadScene(SceneManager.GetSceneByName("Game"));
                }

                SceneManager.SetActiveScene(SceneManager.GetSceneByName("GameEnd"));
            }
        }

        private void RoundManagerOnOnLoadEndGame()
        {
            NetworkManager.Singleton.SceneManager.LoadScene("GameEnd", LoadSceneMode.Additive);
        }

        public override void OnDestroy()
        {
            _roundManager.OnLoadEndGame -= RoundManagerOnOnLoadEndGame;
        }
    }
}