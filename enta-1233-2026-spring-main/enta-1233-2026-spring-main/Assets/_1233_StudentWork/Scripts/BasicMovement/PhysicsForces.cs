using UnityEngine;

public class PhysicsForces : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Vector3 _force;
    [SerializeField] private bool _continuous;
    [SerializeField] private ForceMode _forceMode;

    void Start()
    {
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
        if (_continuous)
        {
            _rigidbody.AddForce(_force, _forceMode);
        }
    }

    private void FixedUpdate()
    {
        if (_continuous)
        {
            _rigidbody.AddForce(_force, _forceMode);
        }
    }
    void Update()
    {
        
    }
}
