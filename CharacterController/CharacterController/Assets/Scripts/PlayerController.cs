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

    // Animation
    [SerializeField] private Animator _animator;
    private static readonly int Speed = Animator.StringToHash("Speed");


    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    public void Move(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
        Debug.Log(_input);

        _direction = new Vector3(_input.x, 0, _input.y);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!_characterController.isGrounded) return;

        _velocity += _jumpPower;
    }

    private void ApplyRotation()
    {
        if (_input.sqrMagnitude == 0) return;

        var targetAngle = Mathf.Atan2(_direction.z * -1, _direction.x) * Mathf.Rad2Deg;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    private void ApplyMovement() 
    {
        _characterController.Move(_direction * speed * Time.deltaTime);
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
        _direction.y = _velocity;
    }
    private void AnimationParameters()
    {
        _animator?.SetFloat(Speed, _input.sqrMagnitude);
    }

    private void Update()
    {
        ApplyRotation();
        ApplyMovement();
        ApplyGravity();
        AnimationParameters();

    }


}
