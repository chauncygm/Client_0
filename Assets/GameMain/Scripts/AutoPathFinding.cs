//AutoPathFinding.cs 挂在角色的gameobject上
using UnityEngine;
using UnityEngine.AI;

public class AutoPathFinding : MonoBehaviour
{
    private NavMeshAgent m_navAgent;
    private Transform m_selfTrans;
    private Vector3 m_targetPos;
    private bool m_isMoving = false;
    
    void Awake()
    {
        // 给角色挂上NavMeshAgent组件
        m_navAgent = gameObject.GetComponent<NavMeshAgent>();
        m_selfTrans = transform;
    }
    
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                m_targetPos = hit.point;
                // 自动寻路移动到目标点
                m_navAgent.SetDestination(m_targetPos);
                m_isMoving = true;
            }
        }
        
        if(Vector3.Distance(m_selfTrans.position, m_targetPos) <= 0.1f)
        {
            m_isMoving = false;
        }
    }
}