using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public sealed class NavMeshAgentMover : MonoBehaviour, IMover
{
    [SerializeField] private NavMeshAgent _agent;

    public Vector3 Velocity => _agent.velocity;
    public float RemainigDistance => _agent.remainingDistance;
    public bool IsAtDestiantion => !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance;

    private void Awake()
    {
        if (_agent == null) _agent = GetComponent<NavMeshAgent>();
    }

    public void SetDestination(Vector3 destination) => _agent?.SetDestination(destination);

    public void Stop()
    {
        if (_agent == null) return;
        _agent.isStopped = true;
        _agent.ResetPath();
    }

    public void Resume()
    {
        if (_agent == null) return;
        _agent.isStopped = false;
    }

    public void SetEnabled(bool value)
    {
        if (_agent == null) return;
        _agent.enabled = value;
    }
}