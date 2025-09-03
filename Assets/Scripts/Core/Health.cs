using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    public float MaxHealth => maxHealth;
    public float Current { get; private set; }
    public bool IsInvulnerable { get; private set; }

    public UnityEvent<float, float> onHealthChanged; // current, max
    public UnityEvent onDie;

    private bool dead;

    private void Awake()
    {
        Current = maxHealth;
        onHealthChanged ??= new UnityEvent<float, float>();
        onDie ??= new UnityEvent();
    }

    public void SetInvulnerable(bool value) => IsInvulnerable = value;

    public void TakeDamage(float amount)
    {
        if (dead || IsInvulnerable) return;
        Current = Mathf.Max(0f, Current - amount);
        onHealthChanged.Invoke(Current, maxHealth);
        if (Current <= 0f) Die();
    }

    public void Heal(float amount)
    {
        if (dead) return;
        Current = Mathf.Min(maxHealth, Current + amount);
        onHealthChanged.Invoke(Current, maxHealth);
    }

    private void Die()
    {
        if (dead) return;
        dead = true;
        onDie.Invoke();
    }
}
