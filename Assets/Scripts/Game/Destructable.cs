﻿using Lobby;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Destructable : NetworkBehaviour, IDamageable
    {
        [SerializeField] private MapPreset preset;
        [SerializeField] private Vector3 dropOffset;
        private NetworkVariable<int> _dropIndex;
        private SpawnerOnGrid _spawnerOnGrid;
        private GameSettings _gameSettings;
        private PowerupSelection _powerupSelection;

        [Inject]
        private void Inject(SpawnerOnGrid spawnerOnGrid, GameSettings gameSettings, PowerupSelection powerupSelection)
        {
            _spawnerOnGrid = spawnerOnGrid;
            _gameSettings = gameSettings;
            _powerupSelection = powerupSelection;
        }

        private void Awake() => _dropIndex = new NetworkVariable<int>();

        public void TakeDamage(int amount, ulong killerId, DeathType deathType) => SpawnDropServerRpc();

        [ServerRpc(RequireOwnership = false)]
        public void SpawnDropServerRpc()
        {
            if (Random.value <= _gameSettings.DropChance)
            {
                _dropIndex.Value = Random.Range(0, preset.Drops.Length);
                _spawnerOnGrid.SpawnInject(preset.Drops[_dropIndex.Value].gameObject, transform.position + dropOffset);
            }

            NetworkObject.Despawn(true);
        }
    }
}