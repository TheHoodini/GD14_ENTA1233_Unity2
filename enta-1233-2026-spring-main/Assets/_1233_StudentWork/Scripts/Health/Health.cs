using System;
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private bool _isInvulnerable;

    [Header("Damage Flash")]
    [SerializeField] private float _damageFlashDuration = 0.3f;
    [SerializeField] private Color _damageFlashColor = Color.red;

    public int CurrentHealth { get; private set; }
    public int MaxHealth => _maxHealth;
    public bool IsDead { get; private set; }

    private Renderer[] _renderers;
    private Color[] _originalColors;
    private Coroutine _flashCoroutine;
    //private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    private void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        _originalColors = new Color[_renderers.Length];
        for (int i = 0; i < _renderers.Length; i++)
            _originalColors[i] = _renderers[i].material.color;

        ResetHealth();
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
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, _maxHealth);
        Debug.Log($"Damage applied: {info.Amount}, Current Health: {CurrentHealth}");

        FlashDamageColor();

        OnDamaged?.Invoke(info);
        if (CurrentHealth <= 0) Die();
    }

    public void Heal(int amount)
    {
        if (IsDead) return;

        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, _maxHealth);
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

    private void FlashDamageColor()
    {
        if (_renderers == null || _renderers.Length == 0) return;

        if (_flashCoroutine != null)
            StopCoroutine(_flashCoroutine);

        _flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        foreach (var r in _renderers)
            r.material.color = _damageFlashColor;

        float elapsed = 0f;
        while (elapsed < _damageFlashDuration)
        {
            float t = elapsed / _damageFlashDuration;
            for (int i = 0; i < _renderers.Length; i++)
                _renderers[i].material.color = Color.Lerp(_damageFlashColor, _originalColors[i], t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < _renderers.Length; i++)
            _renderers[i].material.color = _originalColors[i];

        _flashCoroutine = null;
    }
}