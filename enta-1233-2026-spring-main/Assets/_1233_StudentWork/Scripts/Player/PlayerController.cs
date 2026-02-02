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
            _normalCamera.gameObject.SetActive(false);
            _zoomedCamera.gameObject.SetActive(true);
        }
        else if (context.canceled)
        {
            _zoomedCamera.gameObject.SetActive(false);
            _normalCamera.gameObject.SetActive(true);
        }
    }

    private Transform GetActiveCameraTransform()
    {
        // Return the transform of whichever camera is currently active
        if (_normalCamera.gameObject.activeInHierarchy)
            return _normalCamera.transform;
        else if (_zoomedCamera.gameObject.activeInHierarchy)
            return _zoomedCamera.transform;

        return _normalCamera.transform; // fallback
    }

    private void ApplyRotation()
    {
        if (_input.sqrMagnitude == 0) return;

        var targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    private void ApplyMovement()
    {
        // Get the active camera's transform
        Transform cameraTransform = GetActiveCameraTransform();

        // Get camera's forward and right directions
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Flatten the camera directions (remove y component)
        cameraForward.y = 0;
        cameraRight.y = 0;

        // Normalize to ensure consistent speed
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate movement direction relative to camera
        _direction = cameraForward * _input.y + cameraRight * _input.x;

        // Apply gravity to the Y component
        _direction.y = _velocity;

        // Move the character
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
        ApplyMovement();
        ApplyRotation();
        AnimationParameters();
    }
}