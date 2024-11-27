using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimatorTest : MonoBehaviour
{
    private static readonly int JumpHash = Animator.StringToHash("jump");
    private static readonly int RunHash = Animator.StringToHash("run");
    private static readonly int SpeedScaleHash = Animator.StringToHash("speedScale");
    private static readonly int MoveHash = Animator.StringToHash("move");
    private static readonly int BackHash = Animator.StringToHash("back");
    
    private const float THRESHOLD = 0.1f;
    private const float DEFAULT_SPEED = 1.5f;
    private const int ROTATION_SPEED = 180;

    private Animator _animator;
    private Rigidbody _rigidbody;
    private AnimatorStateInfo _stateInfo;
    private Transform _transform;

    private float _currentSpeed;
    private float _targetSpeed;
    private Vector2 _inputVector2;
    private Vector3 _playerRotation;
    private float _rotateValue;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _transform = transform;
        Debug.Log("Current State loop: " + _stateInfo.loop);
        Debug.Log("human Scale: " + _animator.humanScale);
    }

    private void Update()
    {
        if (Keyboard.current.backspaceKey.isPressed)
        {
            Debug.Log("press the backspace Jump");
            _animator.SetTrigger(JumpHash);
        }
        RotatePlayer();
    }

    private void RotatePlayer()
    {
        if (Mathf.Abs(_rotateValue) < THRESHOLD)
            return;
        
        transform.Rotate(0, _rotateValue * ROTATION_SPEED * Time.deltaTime, 0);
    }

    public void OnAnimatorMove()
    {
        // transform.position += animator.deltaPosition * Time.deltaTime;
        // Debug.Log("velocity: " + _rigidbody.linearVelocity);
        var velocity = new Vector3(_animator.velocity.x, _rigidbody.linearVelocity.y, _animator.velocity.z);
        _rigidbody.linearVelocity = velocity;
        // Debug.Log("" + transform.position);
    }

    public void PlayerMove(InputAction.CallbackContext callback)
    {
        _inputVector2 = callback.ReadValue<Vector2>();
        _rotateValue = _inputVector2.x;
        if (!_inputVector2.Equals(Vector2.zero))
            Debug.Log("inputVector2: " + _inputVector2);

        switch (_inputVector2.y)
        {
            case >= 0 and > THRESHOLD:
                _animator.SetBool(MoveHash, true);
                _animator.SetFloat(SpeedScaleHash, 1 * DEFAULT_SPEED);
                break;
            case >= 0 and <= THRESHOLD:
                _animator.SetBool(MoveHash, false);
                _animator.SetFloat(SpeedScaleHash, 0);
                break;
            case <= 0 and < -THRESHOLD:
                _animator.SetBool(BackHash, true);
                _animator.SetFloat(SpeedScaleHash, -1 * DEFAULT_SPEED);
                break;
            case <= 0 and >= -THRESHOLD:
                _animator.SetBool(BackHash, false);
                _animator.SetFloat(SpeedScaleHash, 0);
                break;
        }
    }
    
    public void PlayerJump(InputAction.CallbackContext callback)
    {
        if (callback.phase == InputActionPhase.Performed)
        {
            Debug.Log("press the jump");
            _animator.SetTrigger(JumpHash);
        }
    }
}
