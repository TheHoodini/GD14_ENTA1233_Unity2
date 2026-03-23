using UnityEngine;

public class SnakeIdleState : EnemyState
{
    private readonly SnakeBrain _brain;

    public SnakeIdleState(SnakeBrain brain, EnemyStateMachine machine) : base(machine)
    {
        _brain = brain;
    }

    public override void Enter()
    {
        _brain.Mover?.Stop();
        _brain.AnimatorDriver.SetSpeed(0);
    }

    public override void Tick()
    {
        var target = _brain.TargetProvider.GetTarget();
        if (target == null) return;

        // Transition to chase once player enters detection range
        if (_brain.Detection.IsTargetInDetectionRange(target))
            Machine.ChangeState(new SnakeChaseState(_brain, Machine));
    }
}