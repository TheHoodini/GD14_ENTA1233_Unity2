using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private bool _isInvulnerable;

    public int CurrentHealth { get; private set; }

    public int MaxHealth => _maxHealth;

    public bool IsDead { get; private set; }

    private void Awake()
    {
        
    }

    public event Action<DamageInfo> OnDamaged;
    public event Action OnDied;
    public event Action OnHealed;
    public event Action OnReset;

    public void ResetHealth()
    {
        CurrentHealth = MaxHealth;
        IsDead = false;
        OnReset?.Invoke();
    }

    public void ApplyDamage(DamageInfo info)
    {
        if (IsDead || _isInvulnerable) return;

        CurrentHealth -= info.Amount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0, _maxHealth);

        OnDamaged?.Invoke(info);

        if (CurrentHealth <= 0) Die();
    }

    public void Heal(int amount)
    {
        if (IsDead) return;

        CurrentHealth += amount;
        CurrentHealth = Mathf.Min(CurrentHealth, 0, _maxHealth);
        OnHealed?.Invoke();
    }

    private void Die()
    {
        IsDead = true;
        OnDied?.Invoke();
    }

    public void SetInvulnerable(bool isInvulnerable)
    {
        _isInvulnerable = isInvulnerable;
    }

}
