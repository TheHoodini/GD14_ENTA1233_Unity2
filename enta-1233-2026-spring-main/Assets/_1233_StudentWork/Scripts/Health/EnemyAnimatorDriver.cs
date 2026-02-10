using UnityEngine;

public class EnemyAnimatorDriver : MonoBehaviour
{
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int AttackTriggerHash = Animator.StringToHash("Attack");
    private static readonly int HitTriggerHash = Animator.StringToHash("Hit");
    private static readonly int DieTriggerHash = Animator.StringToHash("Die");
    [SerializeField] private Animator _animator;

    public void Awake()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
    }

    public void SetSpeed(float speed)
    {
        _animator.SetFloat(SpeedHash, speed);
        _animator.SetBool(IsMovingHash, speed > 0.1f);
    }

    public void TriggerAttack()
    {
        if (_animator == null) return;
        _animator.SetTrigger(AttackTriggerHash);
    }

    public void TriggerHit()
    {
        if (_animator == null) return;
        _animator.SetTrigger(HitTriggerHash);
    }

    public void TriggerDie()
    {
        if (_animator == null) return;
        _animator.SetTrigger(DieTriggerHash);
    }

}
