using Player;
using Unity.Netcode;
using UnityEngine;

namespace Game.Powerups
{
    public abstract class Powerup : NetworkBehaviour
    {
        [SerializeField] private GameObject pickupSound;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerPowerupController player))
            {
                if (player.TryGetComponent(out NetworkObject net))
                {
                    Instantiate(pickupSound, transform.position, Quaternion.identity);
                    PickupServerRpc(net);
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void PickupServerRpc(NetworkObjectReference player)
        {
            if (player.TryGet(out NetworkObject playerNet))
            {
                Apply(playerNet);
                NetworkObject.Despawn();
            }
        }

        protected abstract void Apply(NetworkObject player);
    }
}