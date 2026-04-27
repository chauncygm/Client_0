using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class AutoAdapter : MonoBehaviour
    {
        private Rect _lastSafeArea;
        private Vector2Int _lastScreenSize;
        private CanvasScaler _canvasScaler;
        public Transform safeAreaTransform;
        public bool debug;
        
        public void Awake()
        {
            _canvasScaler = GetComponentInParent<CanvasScaler>();
            ApplySafeArea();
        }

        private static void PrintSafeArea()
        {
            var safeArea = Screen.safeArea;
            Debug.LogWarning("Safe Area: " + safeArea);
            Debug.LogWarning("Screen Size: " + Screen.width + "x" + Screen.height);
            Debug.LogWarning($"position: {safeArea.position}, center: {safeArea.center}, minXY: {safeArea.xMin}, {safeArea.xMax}," +
                             $" {safeArea.yMin}, {safeArea.yMax},  min: {safeArea.min}, {safeArea.max} size: {safeArea.size}");
        }
    
        private void ApplySafeArea()
        {
            _lastSafeArea = Screen.safeArea;
            _lastScreenSize = new Vector2Int(Screen.width, Screen.height);
            
            if (!safeAreaTransform) return;
            var rectTransform = safeAreaTransform.GetComponentInChildren<RectTransform>();
            if (!rectTransform) return;
            
            var safeArea = Screen.safeArea;
            rectTransform.anchorMin = new Vector2(safeArea.xMin / Screen.width, safeArea.yMin / Screen.height);
            rectTransform.anchorMax = new Vector2(safeArea.xMax / Screen.width, safeArea.yMax / Screen.height);
            if (debug)
            {
                PrintSafeArea();
            }

        }

        private void Update()
        {
            var safeAreaChanged = _lastSafeArea != Screen.safeArea;
            var screenSizeChanged = _lastScreenSize != new Vector2Int(Screen.width, Screen.height);
            if (!safeAreaChanged && !screenSizeChanged) return;

            ApplySafeArea();
            UpdateCanvasScaler();
        }

        private void UpdateCanvasScaler()
        {
            if (_canvasScaler == null) return;

            _canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

            // 根据当前分辨率和设计分辨率动态重算匹配策略，避免运行时切换视图后 UI 溢出
            var screenRatio = (float)Screen.width / Screen.height;
            var designRatio = _canvasScaler.referenceResolution.x / _canvasScaler.referenceResolution.y;
            _canvasScaler.matchWidthOrHeight = screenRatio > designRatio ? 1 : 0;
        }
        
#if UNITY_EDITOR
        
        private GUIStyle _safeAreaStyle;
        private const int BorderSize = 5;

        // 创建一个带边框的纹理
        private Texture2D CreateBorderTexture(int borderWidth, Color borderColor, Color fillColor)
        {
            const int size = 64; // 确保最小尺寸
            var texture = new Texture2D(size, size);
            var pixels = new Color[size * size];

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    if (x < borderWidth || x >= size - borderWidth ||
                        y < borderWidth || y >= size - borderWidth)
                    {
                        pixels[y * size + x] = borderColor;
                    }
                    else
                    {
                        pixels[y * size + x] = fillColor;
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.filterMode = FilterMode.Point;
            texture.Apply();
            return texture;
        }
        
        private void OnGUI()
        {
            GUI.depth = 0;
            // 转换 Screen.safeArea 到 GUI 坐标系
            var guiSafeArea = new Rect(
                Screen.safeArea.x,
                Screen.height - Screen.safeArea.yMax,
                Screen.safeArea.width,
                Screen.safeArea.height
            );
            if (_safeAreaStyle == null)
            {
                // 创建一个带边框的 GUIStyle
                _safeAreaStyle = new GUIStyle();
                var borderTexture = CreateBorderTexture(BorderSize, Color.yellow, Color.clear);
                _safeAreaStyle.normal.background = borderTexture;
                _safeAreaStyle.border = new RectOffset(BorderSize ,BorderSize, BorderSize, BorderSize);
            }

            GUI.Box(guiSafeArea, "", _safeAreaStyle);
        }
#endif
    }
}