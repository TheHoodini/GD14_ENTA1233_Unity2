using UnityEngine;

public class HurtZone : MonoBehaviour
{
    [SerializeField] private int _damageAmount = 10;
    private void OnTriggerStay(Collider other)
    {
        var health = other.GetComponent<Health>();
        if (health != null)
        {
            health.ApplyDamage(new DamageInfo
            {
                Amount = _damageAmount,
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

        Gizmos.color = new Color(255f, 128f, 0f, 0.3f);
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.DrawCube(col.center, col.size);

        Gizmos.color = new Color(255f, 128f, 0f, 1f);
        Gizmos.DrawWireCube(col.center, col.size);
    }
}