namespace DungeonLootRush.Combat
{
    public interface IDamageable
    {
        bool IsAlive { get; }
        void ApplyDamage(float amount);
    }
}
