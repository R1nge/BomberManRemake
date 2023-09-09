using Player;
using Unity.Netcode;
using UnityEngine;

namespace Game.Powerups
{
    public class HealthPowerup : Powerup
    {
        [SerializeField] private int amount;
        protected override void Apply(NetworkObject player)
        {
            if (player.TryGetComponent(out PlayerPowerupController powerupController))
            {
                powerupController.HealthPowerup(amount);
            }
        }
    }
}