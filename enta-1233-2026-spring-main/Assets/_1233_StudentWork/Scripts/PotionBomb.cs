using System.Collections;
using UnityEngine;

public class PotionBomb : MonoBehaviour
{
    [SerializeField] private int _potionAmount = 5;
    [SerializeField] private GameObject _potion;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"PotionBomb collided with {other.gameObject.name}");
        var player = other.GetComponentInChildren<PlayerController>();
        if (player == null) return;

        player.BombAmmo += _potionAmount;
        Destroy(_potion);
    }
}