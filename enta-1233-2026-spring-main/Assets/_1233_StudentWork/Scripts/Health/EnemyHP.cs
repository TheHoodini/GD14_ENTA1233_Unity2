using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHP : MonoBehaviour
{
    [SerializeField] private Health _enemyHealth;
    [SerializeField] private Image _healthFillImage;
    private Camera _camera;
    private float _normalizedHealth;


    private void OnEnable()
    {
        if (_enemyHealth == null)
        {
            Debug.LogError("EnemyHP: Enemy Health reference is not set");
            return;
        }
        _enemyHealth.OnHealthChanged += RefreshHealthBar;
        RefreshHealthBar(_enemyHealth);
    }

    private void RefreshHealthBar(Health health)
    {
        if (_healthFillImage == null) return;
        _healthFillImage.fillAmount = health != null ? health.NormalizedHealth : 0f;
    }
}
