using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grenade : MonoBehaviour
{
    [Header("Explosion")]
    [SerializeField] private float _fuseTime = 3f;
    [SerializeField] private float _explosionRadius = 5f;
    [SerializeField] private int _explosionDamage = 50;

    [Header("VFX")]
    [SerializeField] private GameObject _explosionVFX;

    private Rigidbody _rb;
    private GameObject _source;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Launch(Vector3 velocity, GameObject source)
    {
        _source = source;
        _rb.linearVelocity = velocity;
        if (velocity.sqrMagnitude > 0.001f)
            transform.forward = velocity.normalized;

        StartCoroutine(FuseCoroutine());
    }

    private IEnumerator FuseCoroutine()
    {
        yield return new WaitForSeconds(_fuseTime);
        Explode();
    }

    private void Explode()
    {
        // VFX 
        if (_explosionVFX != null)
            Instantiate(_explosionVFX, transform.position, Quaternion.identity);

        // Damage 
        Collider[] hits = Physics.OverlapSphere(transform.position, _explosionRadius);
        foreach (Collider hit in hits)
        {
            if (hit.gameObject == _source) continue;

            var receiver = hit.GetComponent<IDamageReceiver>();
            if (receiver == null) continue;

            var info = new DamageInfo
            {
                Amount = _explosionDamage,
                Source = _source,
                HitPoint = hit.ClosestPoint(transform.position),
                HitNormal = (hit.transform.position - transform.position).normalized
            };
            receiver.ApplyDamage(info);
        }

        Destroy(gameObject);
    }

    // blast radius
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.35f);
        Gizmos.DrawSphere(transform.position, _explosionRadius);
    }
}