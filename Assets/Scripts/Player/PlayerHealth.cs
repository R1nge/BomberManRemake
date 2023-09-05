using System;
using Game;
using Unity.Netcode;
using UnityEngine;

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

        private void Awake()
        {
            _currentHealth = new NetworkVariable<int>(startHealth);
            _currentHealth.OnValueChanged += OnValueChanged;
            _playerShield = GetComponent<PlayerShield>();
        }

        private void Start() => OnInit?.Invoke(_currentHealth.Value);

        public void TakeDamage(int amount)
        {
            if (_playerShield.IsActive)
            {
                _playerShield.TurnOffServerRpc();
                return;
            }

            _currentHealth.Value = Mathf.Clamp(_currentHealth.Value - amount, 0, 100);
        }

        private void OnValueChanged(int _, int health)
        {
            OnDamageTaken?.Invoke(health);

            if (health == 0)
            {
                OnDeath?.Invoke();
            }
        }

        [ServerRpc]
        public void IncreaseHealthServerRpc(int amount)
        {
            _currentHealth.Value += amount;
        }
    }
}