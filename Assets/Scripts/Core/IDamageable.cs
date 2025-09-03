public interface IDamageable
{
    bool IsInvulnerable { get; }
    void TakeDamage(float amount);
}
