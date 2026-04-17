using System.Collections;
using UnityEngine;

public class PotionHealth : MonoBehaviour
{
    [SerializeField] private int _healingAmount = 30;
    [SerializeField] private GameObject _potion;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"PotionHealth collided with {other.gameObject.name}");
        var health = other.GetComponent<Health>();
        if (health == null) return;

        health.Heal(_healingAmount);
        Destroy(_potion);
    }
}