using Unity.Netcode;

namespace Game
{
    public class Bomb : NetworkBehaviour, IDamageable
    {
        public void TakeDamage(int amount)
        {
        }
    }
}