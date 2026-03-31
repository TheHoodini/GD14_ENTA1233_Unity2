using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PatrolMotor : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform[] _destinations;
    
    private int _currentIndex = 0;

    private void Awake()
    {
        if (_destinations.Length > 0)
        {
            _agent.SetDestination(_destinations[_currentIndex].position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_destinations.Length == 0) return;
        if (_agent == null) return;

        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            _currentIndex = (_currentIndex + 1) % _destinations.Length;
            _agent.SetDestination(_destinations[_currentIndex].position);
        }
    }
}
