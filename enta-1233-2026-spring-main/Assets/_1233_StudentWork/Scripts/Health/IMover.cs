using UnityEngine;

public interface IMover
{
    Vector3 Velocity { get; }
    float RemainigDistance { get; }
    bool IsAtDestiantion { get; }
    void SetDestination(Vector3 destination);
    void Stop();
    void Resume();
    void SetEnabled(bool value);
}
