using UnityEngine;

public class SnakeChaseState : EnemyState
{
    private readonly SnakeBrain _brain;

    public void Awake()
    {

    }   
    public SnakeChaseState(SnakeBrain brain, EnemyStateMachine machine) : base(machine)
    {
        _brain = brain;
    }

    public override void Tick()
    {
        var target = _brain.TargetProvider.GetTarget();

        // Lost the player go back to idle
        if (target == null || !_brain.Detection.IsTargetInDetectionRange(target))
        {
            Machine.ChangeState(new SnakeIdleState(_brain, Machine));
            return;
        }

        _brain.Mover?.SetDestination(target.position);
        _brain.AnimatorDriver.SetSpeed(_brain.Mover?.Velocity.magnitude ?? 0f);

        var distance = Vector3.Distance(_brain.transform.position, target.position);
        if (distance <= _brain.AttackRange)
            Machine.ChangeState(new SnakeAttackState(_brain, Machine));
    }
}
