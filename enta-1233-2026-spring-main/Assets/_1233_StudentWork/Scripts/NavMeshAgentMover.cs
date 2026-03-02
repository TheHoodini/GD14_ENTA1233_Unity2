using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public sealed class NavMeshAgentMover : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;

    // public getters for agent related information
    public Vector3 Velocity => _agent.velocity;
    public bool HasPath => _agent.hasPath;

    public void SetDestination(Vector3 worldPos)
    {
        // Best practice: don't spam SetDestination every frame if you don't need to.
        _agent?.SetDestination(worldPos);
    }

    public void Stop()
    {
        _agent?.ResetPath();
    }
}