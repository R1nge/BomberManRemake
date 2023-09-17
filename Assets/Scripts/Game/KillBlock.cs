using Unity.Netcode;
using UnityEngine;

namespace Game
{
    public class KillBlock : NetworkBehaviour
    {
        private const int DAMAGE = 100;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(DAMAGE, 123);
            }
        }
    }
}