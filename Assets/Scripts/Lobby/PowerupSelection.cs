namespace Lobby
{
    public class PowerupSelection
    {
        private bool _bombPowerupEnabled;
        private bool _digPowerupEnabled;
        private bool _healthPowerupEnabled;
        private bool _shieldPowerupEnabled;
        private bool _speedPowerupEnabled;

        public void BombPowerupChangeState(bool newState) => _bombPowerupEnabled = newState;
        public void DigPowerupChangeState(bool newState) => _digPowerupEnabled = newState;
        public void HealthPowerupChangeState(bool newState) => _healthPowerupEnabled = newState;
        public void ShieldPowerupChangeState(bool newState) => _shieldPowerupEnabled = newState;
        public void SpeedPowerupChangeState(bool newState) => _speedPowerupEnabled = newState;

        public bool BombPowerupEnabled => _bombPowerupEnabled;
        public bool DigPowerupEnabled => _digPowerupEnabled;
        public bool HealthPowerupEnabled => _healthPowerupEnabled;
        public bool ShieldPowerupEnabled => _shieldPowerupEnabled;
        public bool SpeedPowerupEnabled => _speedPowerupEnabled;
    }
}