using System;
using Unity.Netcode;

namespace Player
{
    public class PlayerPowerupController : NetworkBehaviour
    {
        private PlayerBombController _playerBombController;
        private PlayerBlockController _playerBlockController;
        private PlayerHealth _playerHealth;
        private PlayerShield _playerShield;
        private PlayerMovement _playerMovement;
        private PlayerMovementTPS _playerMovementTps;

        private void Awake()
        {
            _playerBombController = GetComponent<PlayerBombController>();
            _playerBlockController = GetComponent<PlayerBlockController>();
            _playerHealth = GetComponent<PlayerHealth>();
            _playerShield = GetComponent<PlayerShield>();

            if (TryGetComponent(out PlayerMovement playerMovement))
            {
                _playerMovement = playerMovement;
            }

            if (TryGetComponent(out PlayerMovementTPS playerMovementTps))
            {
                _playerMovementTps = playerMovementTps;
            }
            
        }

        public void BombPowerup(int amount) => _playerBombController.IncreaseBombAmountServerRpc(amount);

        public void DigPowerup(int amount) => _playerBlockController.IncreaseDigServerRpc(amount);

        public void HealthPowerup(int amount) => _playerHealth.IncreaseHealthServerRpc(amount);

        public void ShieldPowerup() => _playerShield.TurnOnServerRpc();

        public void SpeedPowerup(float amount)
        {
            //TODO: create abstract player movement class
            if (_playerMovement != null)
            {
                _playerMovement.IncreaseSpeedServerRpc(amount);
            }

            if (_playerMovementTps != null)
            {
                _playerMovementTps.IncreaseSpeedServerRpc(amount);
            }
        }
    }
}