using Game;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Player
{
    public class SpawnTombstone : NetworkBehaviour
    {
        [SerializeField] private Tombstone tombstone;
        private PlayerHealth _playerHealth;
        private SpawnerOnGrid _spawnerOnGrid;

        [Inject]
        private void Inject(SpawnerOnGrid spawnerOnGrid)
        {
            _spawnerOnGrid = spawnerOnGrid;
        }

        private void Awake()
        {
            _playerHealth = GetComponent<PlayerHealth>();
            _playerHealth.OnDeath += SpawnTombStoneServerRpc;
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnTombStoneServerRpc()
        {
            var position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            var offset = new Vector3(0, transform.position.y - 1, 0);
            _spawnerOnGrid.SpawnInjectWithOwnership(tombstone.gameObject, position, NetworkObject.OwnerClientId, offset);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _playerHealth.OnDeath -= SpawnTombStoneServerRpc;
        }
    }
}