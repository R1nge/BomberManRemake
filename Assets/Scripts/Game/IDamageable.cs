namespace Game
{
    public interface IDamageable
    {
        void TakeDamage(int amount, ulong killerId, DeathType deathType);
    }
}