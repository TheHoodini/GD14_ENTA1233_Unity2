using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var health = other.GetComponent<Health>();
        if (health != null)
        {
            health.ApplyDamage(new DamageInfo
            {
                Amount = health.CurrentHealth,
                Source = gameObject,
                HitPoint = other.ClosestPoint(transform.position),
                HitNormal = (other.transform.position - transform.position).normalized
            });
        }
    }

    private void OnDrawGizmos()
    {
        var col = GetComponent<BoxCollider>();
        if (col == null) return;

        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.DrawCube(col.center, col.size);

        Gizmos.color = new Color(1f, 0f, 0f, 1f);
        Gizmos.DrawWireCube(col.center, col.size);
    }
}