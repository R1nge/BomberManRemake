using System;
using Game;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Player
{
    public class PlayerHealth : NetworkBehaviour, IDamageable
    {
        public event Action<int> OnInit;
        public event Action<int> OnDamageTaken;
        public event Action OnDeath;
        [SerializeField] private int startHealth;
        private NetworkVariable<int> _currentHealth;
        private PlayerShield _playerShield;
        private SpawnerManager _spawnerManager;
        private ulong _killerId;

        [Inject]
        private void Inject(SpawnerManager spawnerManager)
        {
            _spawnerManager = spawnerManager;
        }

        private void Awake()
        {
            _currentHealth = new NetworkVariable<int>(startHealth);
            _currentHealth.OnValueChanged += HealthChanged;
            _playerShield = GetComponent<PlayerShield>();
        }

        private void HealthChanged(int _, int health) => OnDamageTaken?.Invoke(health);

        private void Start() => OnInit?.Invoke(_currentHealth.Value);

        public void TakeDamage(int amount, ulong killerId)
        {
            if (_playerShield.IsActive)
            {
                _playerShield.TurnOffServerRpc();
                return;
            }

            _killerId = killerId;
            _currentHealth.Value = Mathf.Clamp(_currentHealth.Value - amount, 0, 100);
            
            
            if (_currentHealth.Value == 0)
            {
                OnDeath?.Invoke();
                if (IsServer)
                {
                    _spawnerManager.Despawn(NetworkObject.OwnerClientId, _killerId);
                    NetworkObject.Despawn(true);
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void IncreaseHealthServerRpc(int amount) => _currentHealth.Value += amount;
    }
}