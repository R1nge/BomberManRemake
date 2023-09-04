﻿using System;
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

        private void Awake()
        {
            _currentHealth = new NetworkVariable<int>(startHealth);
        }

        private void Start()
        {
            OnInit?.Invoke(_currentHealth.Value);
        }

        public void TakeDamage(int amount)
        {
            _currentHealth.Value = Mathf.Clamp(_currentHealth.Value - amount, 0, 100);

            OnDamageTaken?.Invoke(_currentHealth.Value);

            if (_currentHealth.Value == 0)
            {
                OnDeath?.Invoke();
            }
        }
    }
}