using Unity.Netcode;

namespace Game
{
    public class Destructable : NetworkBehaviour, IDamageable
    {
        public void TakeDamage(int amount) => NetworkObject.Despawn(true);
    }
}