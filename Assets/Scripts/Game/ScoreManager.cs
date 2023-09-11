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
        private RoundManager _roundManager;
        private ulong _lastPlayerId;

        [Inject]
        private void Inject(SpawnerManager spawnerManager, Lobby.Lobby lobby, RoundManager roundManager)
        {
            _spawnerManager = spawnerManager;
            _lobby = lobby;
            _roundManager = roundManager;
        }

        private void Awake()
        {
            _spawnerManager.OnPlayerDeath += AddKillScoreServerRpc;
            _roundManager.OnCleanUpBeforeEnd += AddWinScoreServerRpc;
        }

        [ServerRpc(RequireOwnership = false)]
        private void AddKillScoreServerRpc(ulong killedId, ulong killerId)
        {
            _lastPlayerId = killerId;
            
            if (killedId == killerId)
            {
                return;
            }

            _lobby.AddPoints(killerId, killScore);
        }

        [ServerRpc(RequireOwnership = false)]
        private void AddWinScoreServerRpc()
        {
            _lobby.AddPoints(_lastPlayerId, winScore);
        }

        public override void OnDestroy()
        {
            _spawnerManager.OnPlayerDeath -= AddKillScoreServerRpc;
            _roundManager.OnCleanUpBeforeEnd -= AddWinScoreServerRpc;
        }
    }
}