using UnityEngine;
using UnityEngine.InputSystem;

namespace GameMain.Scripts.test
{
    public class AnimatorTest : MonoBehaviour
    {
        private static readonly int SpeedScaleHash = Animator.StringToHash("speedScale");
        private static readonly int JumpHash = Animator.StringToHash("jump");
        private static readonly int RunHash = Animator.StringToHash("run");
        private static readonly int FireHash = Animator.StringToHash("fire");
    
        private const float Threshold = 0.1f;
        private const float DefaultSpeed = 1.5f;
        private const int RotationSpeed = 180;

        private Animator _animator;
        private Rigidbody _rigidbody;
        private AnimatorStateInfo _stateInfo;

        private float _currentSpeed;
        private float _targetSpeed;
        private Vector2 _inputVector2;
        private Vector3 _playerRotation;
        private float _rotateValue;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
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
            MovePlayer();
            RotatePlayer();
        }
    
        private void MovePlayer()
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, _targetSpeed, 0.5f);
            _animator.SetFloat(SpeedScaleHash, _currentSpeed);
        }

        private void RotatePlayer()
        {
            if (Mathf.Abs(_rotateValue) < Threshold)
                return;
        
            transform.Rotate(0, _rotateValue * RotationSpeed * Time.deltaTime, 0);
        }

        public void OnAnimatorMove()
        {
            // transform.position += animator.deltaPosition * Time.deltaTime;
            // Debug.Log("velocity: " + _rigidbody.linearVelocity);
            var velocity = new Vector3(_animator.velocity.x, _rigidbody.linearVelocity.y, _animator.velocity.z);
            _rigidbody.linearVelocity = velocity;
        }

        public void PlayerMove(InputAction.CallbackContext callback)
        {
            _inputVector2 = callback.ReadValue<Vector2>();
            _rotateValue = _inputVector2.x;
            if (!_inputVector2.Equals(Vector2.zero))
                Debug.Log("inputVector2: " + _inputVector2);

            switch (_inputVector2.y)
            {
                case >= 0 and > Threshold:
                    var run = _animator.GetBool(RunHash);
                    _targetSpeed = run ? 2 * DefaultSpeed : DefaultSpeed;
                    break;
                case >= 0 and <= Threshold:
                    _targetSpeed = 0;
                    break;
                case <= 0 and < -Threshold:
                    _targetSpeed = -DefaultSpeed;
                    break;
                case <= 0 and >= -Threshold:
                    _targetSpeed = 0;
                    break;
            }
        }
    
        public void PlayerJump(InputAction.CallbackContext callback)
        {
            if (callback.phase != InputActionPhase.Performed) return;
            Debug.Log("press the jump");
            _animator.SetTrigger(JumpHash);
        }
    
        public void PlayerFire(InputAction.CallbackContext callback)
        {
            if (callback.phase != InputActionPhase.Performed) return;
            Debug.Log("press the fire");
            _animator.SetTrigger(FireHash);
        }
    
        public void PlayerRun(InputAction.CallbackContext callback)
        {
            var run = callback.phase == InputActionPhase.Performed;
            var changeState = _animator.GetBool(RunHash) != run;
            _animator.SetBool(RunHash, run);
        
            if (_animator.GetFloat(SpeedScaleHash) > Threshold && changeState)
            {
                _targetSpeed = run ? 2 * DefaultSpeed : DefaultSpeed;
            }
        }
    }
}
