using Game;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Player
{
    public class SpawnThumbstone : NetworkBehaviour
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
            _playerHealth.OnDeath += SpawnThumbStonerServerRpc;
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnThumbStonerServerRpc()
        {
            print("SPAWNED A THUMB STONE");
            var position = new Vector3(transform.position.x, transform.position.y - .5f, transform.position.z);
            _spawnerOnGrid.SpawnInjectWithOwnership(tombstone.gameObject, position, NetworkObject.OwnerClientId);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _playerHealth.OnDeath -= SpawnThumbStonerServerRpc;
        }
    }
}