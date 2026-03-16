using System.Collections;
using UnityEngine;

public class SnakeBrain : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private EnemyStateMachine _stateMachine;
    [SerializeField] private DetectionSystem _detection;
    [SerializeField] private EnemyAnimatorDriver _animatorDriver;
    [SerializeField] private RotateToTarget _rotator;
    [SerializeField] private Health _health;

    [Header("Settings")]
    [SerializeField] private float _attackRange = 2f;
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private int _attackDamage = 15;

    [Header("Death Settings")]
    [SerializeField] private float _shrinkDuration = 1f;

    public IMover Mover { get; private set; }

    public DetectionSystem Detection => _detection;
    public EnemyAnimatorDriver AnimatorDriver => _animatorDriver;
    public RotateToTarget Rotator => _rotator;
    public ITargetProvider TargetProvider { get; private set; }

    public float AttackRange => _attackRange;
    public float AttackCooldown => _attackCooldown;
    public int AttackDamage => _attackDamage;

    public bool IsDead { get; private set; }

    private void Awake()
    {
        TargetProvider = GetComponent<ITargetProvider>();
        Mover = GetComponent<IMover>();

        if (_stateMachine == null)
            _stateMachine = GetComponent<EnemyStateMachine>();
    }

    private void Start()
    {
        _stateMachine.Initialize(new SnakeIdleState(this, _stateMachine));
    }

    private void OnEnable()
    {
        if (_health != null)
            _health.OnDied += HandleDied;
    }

    private void HandleDied()
    {
        if (IsDead) return;

        IsDead = true;

        // Prevent any state changes
        _stateMachine.ChangeState(null);

        if (Mover != null)
        {
            Mover.Stop();
            Mover.SetEnabled(false);
        }

        if (_health != null)
        {
            _health.enabled = false;
        }

        _animatorDriver.TriggerDie();
    }

    public void ShrinkAndDestroy()
    {
        StartCoroutine(ShrinkRoutine());
    }

    private IEnumerator ShrinkRoutine()
    {
        Vector3 startScale = transform.localScale;
        float timer = 0f;

        while (timer < _shrinkDuration)
        {
            timer += Time.deltaTime;
            float t = timer / _shrinkDuration;

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

            yield return null;
        }

        Destroy(gameObject);
    }
}