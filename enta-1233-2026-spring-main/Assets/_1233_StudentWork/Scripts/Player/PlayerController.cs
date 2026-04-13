using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private bool _isDead;

    // Move input
    private Vector2 _input;
    private CharacterController _characterController;
    private Vector3 _direction;

    // Movement parameters
    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float smoothTime = 0.05f;
    private float _currentVelocity;
    private bool _isRunning;

    // Gravity
    private float _gravity = -9.81f;
    [SerializeField] private float gravityMult = 3.0f;
    private float _velocity;

    // Jump
    [SerializeField] private float _jumpPower = 2.0f;
    private bool _jumped;

    // Animation
    [SerializeField] private Animator _animator;
    private static readonly int Speed = Animator.StringToHash("Speed");

    // Attack
    [Header("Attack")]
    [SerializeField] private Transform _muzzle;

    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private float _fireRate = 1f;
    private float _nextFireTime;
    [SerializeField] private int _bombAmmo = 5;
    public event System.Action<int> OnBombAmmoChanged;
    public int BombAmmo { 
        get { return _bombAmmo;}
        set { _bombAmmo = Mathf.Clamp(value, 0, 99); 
            OnBombAmmoChanged?.Invoke(_bombAmmo);
        }
    }
      
    [SerializeField] private Grenade _grenadePrefab;
    [SerializeField] private float _grenadeThrowForce = 12f;
    [SerializeField] private float _grenadeArcAngle = 30f;   // degrees

    // Cameras
    [Header("Cameras")]
    [SerializeField] private CinemachineCamera _normalCamera;
    [SerializeField] private CinemachineCamera _zoomedCamera;
    private bool _isZoomed;
    public bool IsZoomed => _isZoomed;
    private bool _wasZoomed;
    public event System.Action<bool> OnZoomChanged;

    [SerializeField] private Health _health;

    private bool _isAttacking;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        if (_health == null) _health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        if (_health != null)
        {
            _health.OnDamaged += HandleDamaged;
            _health.OnDied += HandleDied;
        }
    }

    private void OnDisable()
    {
        if (_health != null)
        {
            _health.OnDamaged -= HandleDamaged;
            _health.OnDied -= HandleDied;
        }
    }

    private void HandleDamaged(DamageInfo info)
    {
        Debug.Log($"[Player] Hit by " + $"{info.Source?.name ?? "Unknown"} " + $"for {info.Amount} damage. " + $"HP: {_health.CurrentHealth}/{_health.MaxHealth}");
        _animator?.SetTrigger("Hit");
    }

    private void HandleDied()
    {
        Debug.Log("[Player] Died!");
        _isDead = true;

        _animator?.ResetTrigger("Hit");

        _animator?.SetTrigger("Die");
        _characterController.enabled = false;

        StartCoroutine(GameOverTransition());
    }

    private IEnumerator GameOverTransition()
    {
        yield return new WaitForSeconds(2);
        enabled = false;
        GameMgr.Instance.GameOver();
    }


    public void Move(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
        //Debug.Log(_input);
    }

    public void Run(InputAction.CallbackContext context)
    {
        if (_isZoomed) return; 
        _isRunning = context.ReadValueAsButton();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!_characterController.isGrounded) return;
        if (_jumped) return;

        _jumped = true;
        _velocity += _jumpPower;
        //Debug.Log("Jumped");
    }
    // ----------------------------------------------------------------------------------
    // ATTACK
    // ----------------------------------------------------------------------------------
    public void Attack(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!_characterController.isGrounded) return;
        if (_isAttacking) return;

        _isAttacking = true;
        _animator?.SetTrigger("IsAttacking");

    }
    public void OnAttackAnimationEnd()
    {
        _isAttacking = false;
        if (_isZoomed)
        {
            // shoot with camera
            Vector3 cameraForward = _zoomedCamera.transform.forward;
            Fire(cameraForward.normalized);
        }
        else
        {
            // shoot straight forward 
            Fire(transform.forward);
        }
    }

    public void Fire(Vector3 direction)
    {
        //if (Time.time < _nextFireTime) return;
        _nextFireTime = Time.time + 1f / _fireRate;
        SpawnProjectile(direction);
    }

    private void SpawnProjectile(Vector3 direction)
    {
        var projectile = Instantiate(_projectilePrefab, _muzzle.position,
            Quaternion.LookRotation(direction));
        projectile.Launch(direction, gameObject);
    }

    public void AttackGrenade(InputAction.CallbackContext context)
    {
        Debug.Log("Grenade");
        if (BombAmmo <= 0) return;
        if (!context.started) return;
        if (!_characterController.isGrounded) return;
        if (_isAttacking) return;
        if (_grenadePrefab == null) return;

        //_animator?.SetTrigger("IsAttacking");
        OnBombAmmoChanged?.Invoke(--BombAmmo);
        ThrowGrenade();
    }

    private void ThrowGrenade()
    {
        Vector3 forward = _isZoomed
            ? _zoomedCamera.transform.forward
            : transform.forward;

        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
        Vector3 throwDir = Quaternion.AngleAxis(-_grenadeArcAngle, right) * forward;

        var grenade = Instantiate(_grenadePrefab, _muzzle.position, Quaternion.identity);
        grenade.Launch(throwDir.normalized * _grenadeThrowForce, gameObject);
    }

    // ----------------------------------------------------------------------------------
    // CAMERA
    // ----------------------------------------------------------------------------------
    public void CameraZoom(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _wasZoomed = true;
            _isRunning = false;
            var orbital = _zoomedCamera.GetComponent<CinemachineOrbitalFollow>();
            if (orbital != null)
            {
                orbital.HorizontalAxis.Value = transform.eulerAngles.y;
            }
            _normalCamera.gameObject.SetActive(false);
            _zoomedCamera.gameObject.SetActive(true);
            _isZoomed = true;
        }
        else if (context.canceled)
        {
            // remove orbital snap here
            _zoomedCamera.gameObject.SetActive(false);
            _normalCamera.gameObject.SetActive(true);
            _isZoomed = false;
        }
        OnZoomChanged?.Invoke(_isZoomed);
    }

    private Transform GetActiveCameraTransform()
    {
        if (_normalCamera.gameObject.activeInHierarchy)
            return _normalCamera.transform;
        else if (_zoomedCamera.gameObject.activeInHierarchy)
            return _zoomedCamera.transform;

        return _normalCamera.transform; 
    }

    private void ApplyRotation()
    {
        if (_isZoomed)
        {
            // look forward when zoomed
            Vector3 cameraForward = _zoomedCamera.transform.forward;
            cameraForward.y = 0;

            if (cameraForward.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(cameraForward);
            }
            return;
        }

        // snap forward when unzoomed
        if (_wasZoomed)
        {
            _wasZoomed = false;
            var orbital = _normalCamera.GetComponent<CinemachineOrbitalFollow>();
            if (orbital != null)
            {
                orbital.HorizontalAxis.Value = transform.eulerAngles.y;
            }
        }

        if (_input.sqrMagnitude == 0) return;

        var targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    // ----------------------------------------------------------------------------------
    // MOVEMENT & GRAVITY
    // ----------------------------------------------------------------------------------

    private void ApplyMovement()
    {
        Transform cameraTransform = GetActiveCameraTransform();

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        _direction = cameraForward * _input.y + cameraRight * _input.x;

        _direction.y = _velocity;

        if (_isAttacking) return;

        float _currentSpeed = speed * (_isRunning ? 2f : 1f);
        _characterController.Move(_direction * _currentSpeed * Time.deltaTime);

        if (_characterController.isGrounded)
        {
            _jumped = false;
        }
    }

    private void ApplyGravity()
    {
        if (_characterController.isGrounded && _velocity < 0.0f)
        {
            _velocity = -1.0f;
        }
        else
        {
            _velocity += _gravity * gravityMult * Time.deltaTime;
        }
    }

    private void AnimationParameters()
    {
        if (_isDead) return;

        float animSpeed = 0f;
        if (_input.sqrMagnitude > 0)
            animSpeed = _isRunning ? 2f : 1f;

        _animator?.SetFloat(Speed, animSpeed);
        _animator?.SetBool("Jumped", _jumped);
        _animator?.SetBool("IsGrounded", _characterController.isGrounded);
    }

    private void Update()
    {
        ApplyGravity();

        if (_isDead) return;

        ApplyRotation();
        ApplyMovement();
        AnimationParameters();
    }
}