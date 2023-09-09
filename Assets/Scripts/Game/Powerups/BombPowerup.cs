using Player;
using Unity.Netcode;
using UnityEngine;

namespace Game.Powerups
{
    public class BombPowerup : Powerup
    {
        [SerializeField] private int bombAmount;

        protected override void Apply(NetworkObject player)
        {
            if (player.TryGetComponent(out PlayerPowerupController powerupController))
            {
                powerupController.BombPowerup(bombAmount);
            }
        }
    }
}