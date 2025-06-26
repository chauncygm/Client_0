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

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            // 获取 Canvas 的 Rect
            _canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            // 计算屏幕中心的世界坐标
            _screenCenter = _canvasRect.rect.center;
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            
            if (loading == null)
                return;

            // 获取当前 loading 的位置
            Vector2 currentPos = loading.rectTransform.position;

            // 计算到屏幕中心的方向向量
            var direction = (Vector2)_canvasRect.position + _screenCenter - currentPos;

            // 计算朝向角度（弧度转角度，并偏移 90 度以适配 UI 默认朝向）
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

            // 设置 rotation
            loading.rectTransform.rotation = Quaternion.Euler(0f, 0f, angle);

        }
    }
}