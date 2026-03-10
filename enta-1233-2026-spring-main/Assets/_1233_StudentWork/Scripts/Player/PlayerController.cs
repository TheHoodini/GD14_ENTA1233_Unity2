using System.Collections;
using Unity.Cinemachine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
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

    // Cameras
    [Header("Cameras")]
    [SerializeField] private CinemachineCamera _normalCamera;
    [SerializeField] private CinemachineCamera _zoomedCamera;
    private bool _isZoomed;

    [SerializeField] private Health _health;

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
        _animator?.SetTrigger("Die");
        _characterController = null;
        enabled = false;
        StartCoroutine(GameOverTransition());
    }

    private IEnumerator GameOverTransition()
    {
        yield return new WaitForSeconds(2);
        GameMgr.Instance.GameOver();
    }


    public void Move(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
        //Debug.Log(_input);
    }

    public void Run(InputAction.CallbackContext context)
    {
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

    public void CameraZoom(InputAction.CallbackContext context)
    {
        if (context.started)
        {

            _normalCamera.gameObject.SetActive(false);
            _zoomedCamera.gameObject.SetActive(true);
            _isZoomed = true;
        }
        else if (context.canceled)
        {
            _zoomedCamera.gameObject.SetActive(false);
            _normalCamera.gameObject.SetActive(true);
            _isZoomed = false;
        }
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
        if (_input.sqrMagnitude == 0) return;

        // look forward when zoomed
        if (_isZoomed)
        {
            Vector3 cameraForward = _zoomedCamera.transform.forward;
            cameraForward.y = 0;

            if (cameraForward.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
                transform.rotation = targetRotation;
            }
            return;
        }

        var targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }



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
        ApplyRotation();
        ApplyMovement();
        AnimationParameters();
    }
}