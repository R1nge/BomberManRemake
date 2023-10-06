using Game.StateMachines;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Game
{
    public class ScoreManager : NetworkBehaviour
    {
        [SerializeField] private int killScore, winScore;
        private SpawnerManager _spawnerManager;
        private Lobby.Lobby _lobby;
        private GameStateController2 _gameStateController2;

        [Inject]
        private void Inject(SpawnerManager spawnerManager, Lobby.Lobby lobby, GameStateController2 gameStateController)
        {
            _spawnerManager = spawnerManager;
            _lobby = lobby;
            _gameStateController2 = gameStateController;
        }

        private void Awake()
        {
            _spawnerManager.OnPlayerDeath += AddKillScoreServerRpc;
            _gameStateController2.OnStateChanged += StateChanged;
        }

        private void StateChanged(GameStates newStates)
        {
            switch (newStates)
            {
                case GameStates.Win:
                    AddWinScore();
                    break;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void AddKillScoreServerRpc(ulong killedId, ulong killerId, DeathType deathType)
        {
            if (deathType == DeathType.Map)
            {
                Debug.Log("Was killed by the map, no score will be added");
                return;
            }
            
            if (killedId == killerId)
            {
                Debug.Log("Killed himself, no score will be added");
                return;
            }

            _lobby.AddPoints(killerId, killScore);

            Debug.LogError($"ADD KILL POINTS TO {killerId}");
        }

        private void AddWinScore()
        {
            if (!IsServer) return;
            if (_spawnerManager.PlayersAliveCount > 1)
            {
                Debug.LogError("The game ended in a tie, no points will be added");
                return;
            }

            _lobby.AddPoints(_spawnerManager.LastPlayerAlive, winScore);
            Debug.LogError($"ADD WIN POINTS TO {_spawnerManager.LastPlayerAlive}");
        }

        public override void OnDestroy()
        {
            _spawnerManager.OnPlayerDeath -= AddKillScoreServerRpc;
            _gameStateController2.OnStateChanged -= StateChanged;
        }
    }
}