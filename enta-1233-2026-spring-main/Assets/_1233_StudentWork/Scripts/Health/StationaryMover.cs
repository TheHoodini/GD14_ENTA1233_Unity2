using UnityEngine;

public class StationaryMover : MonoBehaviour
{
    public Vector3 Velocity => Vector3.zero;
    public float RemainingDistance => 0f;
    public bool IsAtDestination => true;

    public void SetDestination(Vector3 destination)
    {
        // pass
    }

    public void Stop()
    {
        // pass
    }

    public void Resume()
    {
        // pass
    }

    public void SetEnabled(bool value)
    {
        // pass
    }
}
