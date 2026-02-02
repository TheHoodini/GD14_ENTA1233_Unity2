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
    [SerializeField] private float speed;
    [SerializeField] private float smoothTime = 0.05f;
    private float _currentVelocity;

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

    // Camera
    [SerializeField] private CinemachineCamera _normalCamera;
    [SerializeField] private CinemachineCamera _zoomedCamera;
    private bool _isZoomed;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    public void Move(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
        Debug.Log(_input);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!_characterController.isGrounded) return;
        if (_jumped) return;

        _jumped = true;
        _velocity += _jumpPower;
        Debug.Log("Jumped");
    }

    public void CameraZoom(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            //_zoomedCamera.transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

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

        _characterController.Move(_direction * speed * Time.deltaTime);

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
        _animator?.SetFloat(Speed, _input.sqrMagnitude);
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