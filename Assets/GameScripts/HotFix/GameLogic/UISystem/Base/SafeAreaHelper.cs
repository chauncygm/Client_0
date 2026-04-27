using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 安全区适配工具类。
    /// 提供安全区锚点计算与变化检测，供 UISystem 和 UIWindow 使用。
    /// </summary>
    public static class SafeAreaHelper
    {
        private static Rect _lastSafeArea;
        private static Vector2Int _lastScreenSize;
        private static bool _initialized;

        /// <summary>
        /// 将 RectTransform 的锚点对齐到当前屏幕安全区。
        /// </summary>
        /// <param name="rectTransform">需要适配的 RectTransform。</param>
        public static void Apply(RectTransform rectTransform)
        {
            if (rectTransform == null) return;

            var safeArea = Screen.safeArea;
            rectTransform.anchorMin = new Vector2(safeArea.xMin / Screen.width, safeArea.yMin / Screen.height);
            rectTransform.anchorMax = new Vector2(safeArea.xMax / Screen.width, safeArea.yMax / Screen.height);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        /// <summary>
        /// 检测安全区是否发生变化（屏幕旋转、分屏等场景）。
        /// 仅当安全区或屏幕尺寸改变时返回 true。
        /// </summary>
        /// <returns>安全区是否发生变化。</returns>
        public static bool CheckSafeAreaChanged()
        {
            var safeArea = Screen.safeArea;
            var screenSize = new Vector2Int(Screen.width, Screen.height);

            if (_initialized && safeArea == _lastSafeArea && screenSize == _lastScreenSize)
                return false;

            _lastSafeArea = safeArea;
            _lastScreenSize = screenSize;
            _initialized = true;
            return true;
        }
    }
}
