using Player;
using Unity.Netcode;

namespace Game.Powerups
{
    public class ShieldPowerup : Powerup
    {
        protected override void Apply(NetworkObject player)
        {
            if (player.TryGetComponent(out PlayerPowerupController powerupController))
            {
                powerupController.ShieldPowerup();
            }
        }
    }
}