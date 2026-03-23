using System.Collections;
using UnityEngine;

public class BluePotion : MonoBehaviour
{
    [SerializeField] private float _invulnerableSeconds = 5f;
    [SerializeField] private GameObject _potion;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"BluePotion collided with {other.gameObject.name}");
        var health = other.GetComponent<Health>();
        if (health == null) return;

        health.StartCoroutine(ApplyInvulnerability(health));
        Destroy(_potion);
    }

    private IEnumerator ApplyInvulnerability(Health health)
    {
        health.SetInvulnerable(true);
        yield return new WaitForSeconds(_invulnerableSeconds);
        health.SetInvulnerable(false);
    }
}