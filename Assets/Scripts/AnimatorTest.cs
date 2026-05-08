using UnityEngine;
using UnityEngine.InputSystem;

namespace GameMain.Scripts.test
{
    public class AnimatorTest : MonoBehaviour, Client_0.IPlayerActions
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
        private Client_0 _inputActions;

        private float _currentSpeed;
        private float _targetSpeed;
        private Vector2 _inputVector2;
        private Vector3 _playerRotation;
        private float _rotateValue;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
            
            // 启用根运动，让 Animator 计算移动速度
            _animator.applyRootMotion = true;
            
            // 创建输入动作实例
            _inputActions = new Client_0();
            
            // 注册回调接口
            _inputActions.Player.AddCallbacks(this);
        }

        private void Start()
        {
            Debug.Log("human Scale: " + _animator.humanScale);
        }

        private void OnEnable()
        {
            // 启用玩家输入
            _inputActions.Player.Enable();
        }

        private void OnDisable()
        {
            // 禁用玩家输入
            _inputActions.Player.Disable();
        }

        private void OnDestroy()
        {
            // 释放资源
            _inputActions.Dispose();
        }

        private void Update()
        {
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
            // 应用根运动到 Rigidbody
            var velocity = new Vector3(_animator.velocity.x, _rigidbody.velocity.y, _animator.velocity.z);
            _rigidbody.velocity = velocity;
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

        public void OnMove(InputAction.CallbackContext context)
        {
            PlayerMove(context);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Debug.Log("look");
            // do nothing
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            PlayerFire(context);
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            PlayerRun(context);
        }
    }
}
