using Unity.Netcode;
using Zenject;

namespace Game
{
    public class GamePlayerManager : NetworkBehaviour
    {
        private int _playersAlive;
        private SpawnerManager _spawnerManager;
        private GameStateController _gameStateController;

        [Inject]
        private void Inject(SpawnerManager spawnerManager, GameStateController gameStateController)
        {
            _spawnerManager = spawnerManager;
            _gameStateController = gameStateController;
        }

        private void Awake()
        {
            _spawnerManager.OnPlayerSpawn += IncreaseServerRpc;
            _spawnerManager.OnPlayerDeath += DecreaseServerRpc;
            _gameStateController.OnRoundEnded += Reset;
        }

        private void Reset()
        {
            if (IsServer)
            {
                _playersAlive = 0;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void IncreaseServerRpc(ulong clientId)
        {
            _playersAlive++;
            print($"Players alive: {_playersAlive}");
        }

        [ServerRpc(RequireOwnership = false)]
        private void DecreaseServerRpc(ulong killedId, ulong killerId)
        {
            _playersAlive--;
            print($"REMOVED: Players alive: {_playersAlive}");
            if (_playersAlive <= 1)
            {
                _gameStateController.EndGame();
            }
        }

        public override void OnDestroy()
        {
            _spawnerManager.OnPlayerSpawn -= IncreaseServerRpc;
            _spawnerManager.OnPlayerDeath -= DecreaseServerRpc;
            _gameStateController.OnRoundEnded -= Reset;
        }
    }
}