using System;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace GameMain.Scripts.UI
{
    public class WaitingPanel : UIFormLogic
    {
        
        [SerializeField] private Image loading;
        [SerializeField] private float rotateSpeed = 180f;
        
        private RectTransform _canvasRect; // Canvas 的 Rect
        private Vector2 _screenCenter;     // 屏幕中心的世界坐标
        private float currentAngle;     // 当前旋转角度

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            // 获取 Canvas 的 Rect
            _canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            // 计算屏幕中心的世界坐标
            var centerLocal = _canvasRect.rect.center;
            _screenCenter = _canvasRect.TransformPoint(centerLocal); // 转换为世界坐标
        }
        
        
        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            
            if (loading == null)
                return;
            
            // 每帧根据时间增量更新角度（rotateSpeed = 180°/s）
            currentAngle += rotateSpeed * Time.deltaTime;
            currentAngle %= 360f; // 防止溢出

            // 定义旋转半径
            const float radius = 15f; // 可调：围绕中心的距离

            // 计算新的位置（极坐标转笛卡尔坐标）
            var deg2Rad = currentAngle * Mathf.Deg2Rad;
            var newPosition = _screenCenter + new Vector2(Mathf.Cos(deg2Rad), Mathf.Sin(deg2Rad)) * radius;

            // 设置 loading 的位置
            loading.rectTransform.position = newPosition;

            // （可选）设置 loading 的朝向始终指向中心
            var direction = _screenCenter - (Vector2)loading.rectTransform.position;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            loading.rectTransform.rotation = Quaternion.Euler(0f, 0f, angle);

        }
    }
}