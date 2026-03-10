using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private Transform[] patrolPoints;
    private NavMeshAgent _agent;
    private int _currentIndex = 0;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        if (patrolPoints.Length > 0)
        {
            _agent.SetDestination(patrolPoints[_currentIndex].position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (patrolPoints.Length == 0) return;

        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            _currentIndex = (_currentIndex + 1) % patrolPoints.Length;
            _agent.SetDestination(patrolPoints[_currentIndex].position);
        }
    }
}
