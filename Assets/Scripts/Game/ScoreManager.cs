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
        private NetworkVariable<ulong> _lastPlayerId;

        [Inject]
        private void Inject(SpawnerManager spawnerManager, Lobby.Lobby lobby, RoundManager roundManager)
        {
            _spawnerManager = spawnerManager;
            _lobby = lobby;
            _roundManager = roundManager;
        }

        private void Awake()
        {
            _lastPlayerId = new NetworkVariable<ulong>();
            _spawnerManager.OnPlayerDeath += AddKillScoreServerRpc;
            _roundManager.OnCleanUpBeforeEnd += AddWinScore;
        }

        [ServerRpc(RequireOwnership = false)]
        private void AddKillScoreServerRpc(ulong killedId, ulong killerId)
        {
            _lastPlayerId.Value = killerId;

            if (killedId == killerId)
            {
                return;
            }

            _lobby.AddPoints(killerId, killScore);

            Debug.LogError($"ADD KILL POINTS TO {killerId}");
        }

        private void AddWinScore()
        {
            if (!IsServer) return;
            _lobby.AddPoints(_lastPlayerId.Value, winScore);
            Debug.LogError($"ADD WIN POINTS TO {_lastPlayerId.Value}");
        }

        public override void OnDestroy()
        {
            _spawnerManager.OnPlayerDeath -= AddKillScoreServerRpc;
            _roundManager.OnCleanUpBeforeEnd -= AddWinScore;
        }
    }
}