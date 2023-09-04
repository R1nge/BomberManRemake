using System;
using Unity.Netcode;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Game
{
    public class Destructable : NetworkBehaviour, IDamageable
    {
        [SerializeField] private MapPreset preset;
        [SerializeField] private NetworkVariable<float> dropChance;
        [SerializeField] private Vector3 dropOffset;
        private NetworkVariable<int> _dropIndex;
        private SpawnerOnGrid _spawnerOnGrid;

        [Inject]
        private void Inject(SpawnerOnGrid spawnerOnGrid)
        {
            _spawnerOnGrid = spawnerOnGrid;
        }

        private void Awake()
        {
            _dropIndex = new NetworkVariable<int>();
        }

        public void TakeDamage(int amount)
        {
            SpawnDropServerRpc();
            NetworkObject.Despawn(true);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnDropServerRpc()
        {
            if (Random.value < 1 - dropChance.Value) return;
            _dropIndex.Value = Random.Range(0, preset.Drops.Length);
            _spawnerOnGrid.SpawnInject(preset.Drops[_dropIndex.Value].gameObject, transform.position + dropOffset);
        }
    }
}