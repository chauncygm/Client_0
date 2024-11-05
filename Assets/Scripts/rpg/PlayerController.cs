using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject playerObject;

    public float moveSpeed = 3.0f; // 移动速度
    public float lookSensitivity = 2.0f; // 鼠标灵敏度

    private Transform cameraTransform; // 摄像机的 Transform
    private float verticalRotation = 0f; // 垂直旋转角度
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraTransform = GetComponentInChildren<Camera>().transform;
    }

    // Update is called once per frame
    void Update()
    { 
        float horizontal = 0f;
        float vertical = 0f;

        // 检测键盘输入
        if (Input.GetKey(KeyCode.W))
        {
            vertical += 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            vertical -= 1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            horizontal -= 1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            horizontal += 1f;
        }

        // 计算移动向量
        Vector3 moveDirection = (transform.forward * vertical + transform.right * horizontal).normalized;
        Vector3 movement =moveDirection * moveSpeed * Time.deltaTime;

        // 更新玩家位置
        transform.Translate(movement, Space.World);

        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;
        
        // 限制摄像机的垂直旋转角度
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -25, 25);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        
        transform.Rotate(0, mouseX, 0);
    }
}
