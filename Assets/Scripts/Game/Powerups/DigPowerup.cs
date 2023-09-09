using Player;
using Unity.Netcode;
using UnityEngine;

namespace Game.Powerups
{
    public class DigPowerup : Powerup
    {
        [SerializeField] private int amount;

        protected override void Apply(NetworkObject player)
        {
            if (player.TryGetComponent(out PlayerPowerupController powerupController))
            {
                powerupController.DigPowerup(amount);
            }
        }
    }
}