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

        private void Awake()
        {
            _playerBombController = GetComponent<PlayerBombController>();
            _playerBlockController = GetComponent<PlayerBlockController>();
            _playerHealth = GetComponent<PlayerHealth>();
            _playerShield = GetComponent<PlayerShield>();
            _playerMovement = GetComponent<PlayerMovement>();
        }

        public void BombPowerup(int amount) => _playerBombController.IncreaseBombAmountServerRpc(amount);

        public void DigPowerup(int amount) => _playerBlockController.IncreaseDigAmountServerRpc(amount);

        public void HealthPowerup(int amount) => _playerHealth.IncreaseHealthServerRpc(amount);

        public void ShieldPowerup() => _playerShield.TurnOnServerRpc();

        public void SpeedPowerup(float amount) => _playerMovement.IncreaseSpeedServerRpc(amount);
    }
}