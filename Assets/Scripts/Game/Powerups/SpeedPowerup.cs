using Player;
using Unity.Netcode;
using UnityEngine;

namespace Game.Powerups
{
    public class SpeedPowerup : Powerup
    {
        [SerializeField] private float additionalSpeed;

        protected override void Apply(NetworkObject player)
        {
            if (player.TryGetComponent(out PlayerPowerupController powerupController))
            {
                powerupController.SpeedPowerup(additionalSpeed);
            }
        }
    }
}