using Unity.Netcode;
using UnityEngine;

namespace Game
{
    public class KillBlock : NetworkBehaviour
    {
        private const int DAMAGE = 100;

        private void OnCollisionEnter(Collision collision)
        {
            var transform = collision.transform;
            if (transform.TryGetComponent(out NetworkObject networkObject))
            {
                if (transform.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(DAMAGE, networkObject.OwnerClientId, DeathType.Map);
                }
            }
            
        }
    }

    public enum DeathType
    {
        Map,
        Player,
    }
}