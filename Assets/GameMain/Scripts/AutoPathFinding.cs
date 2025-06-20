//AutoPathFinding.cs 挂在角色的gameobject上

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace GameMain.Scripts
{
    public class AutoPathFinding : MonoBehaviour
    {
        private NavMeshAgent _mNavAgent;
        private Transform _mSelfTrans;
        private Vector3 _mTargetPos;
        private bool _mIsMoving;

        private void Awake()
        {
            // 给角色挂上NavMeshAgent组件
            _mNavAgent = gameObject.GetComponent<NavMeshAgent>();
            _mSelfTrans = transform;
        }

        private void Update()
        {
            if(Mouse.current.leftButton.wasPressedThisFrame)
            {
                var mousePosition = Mouse.current.position.ReadValue();
                var ray = Camera.main!.ScreenPointToRay(mousePosition);
                if(Physics.Raycast(ray, out var hit))
                {
                    _mTargetPos = hit.point;
                    // 自动寻路移动到目标点
                    _mNavAgent.SetDestination(_mTargetPos);
                    _mIsMoving = true;
                }
            }
        
            if(Vector3.Distance(_mSelfTrans.position, _mTargetPos) <= 0.1f)
            {
                _mIsMoving = false;
            }
        }
    }
}