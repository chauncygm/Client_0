//AutoPathFinding.cs 挂在角色的gameobject上
using UnityEngine;
using UnityEngine.AI;

namespace GameMain.Scripts
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AutoPathFinding : MonoBehaviour
    {
        private NavMeshAgent _mNavAgent;
        private Vector3 _mTargetPos;
        private Client_0 _mInput;
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
            // 给角色挂上NavMeshAgent组件
            _mNavAgent = gameObject.GetComponent<NavMeshAgent>();
        }

        private void OnEnable()
        {
            _mInput = new Client_0();
            _mInput.UI.Enable();
        }

        private void Update()
        {

            // if(Mouse.current.leftButton.wasPressedThisFrame)
            if (_mInput.UI.Click.WasPressedThisFrame())
            {
                var mousePosition = _mInput.UI.Point.ReadValue<Vector2>();
                Debug.Log($"点击移动: {mousePosition}");
                var ray = _camera!.ScreenPointToRay(mousePosition);
                if(Physics.Raycast(ray, out var hit))
                {
                    _mTargetPos = hit.point;
                    // 自动寻路移动到目标点
                    _mNavAgent.SetDestination(_mTargetPos);
                }
            }
        }
    }
}