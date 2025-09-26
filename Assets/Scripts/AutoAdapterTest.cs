using System;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    [RequireComponent(typeof(CanvasScaler))]
    public class AutoAdapterTest : MonoBehaviour
    {

        private Rect _lastSafeArea;
        private readonly GUIStyle _safeAreaStyle = new();
        private const int SafeAreaBorderSize = 8;

        private CanvasScaler _canvasScaler;
        private Texture2D _borderTexture;
        
        [SerializeField, Header("宽屏基准宽高比例")]
        public float wideScreenSize = 1.67f;
        
        public void Awake()
        {
            // 创建一个带边框的 GUIStyle
            _borderTexture = CreateBorderTexture(SafeAreaBorderSize, Color.yellow, Color.clear);
            _safeAreaStyle.normal.background = _borderTexture;
            _safeAreaStyle.border = new RectOffset(SafeAreaBorderSize ,SafeAreaBorderSize, SafeAreaBorderSize, SafeAreaBorderSize);

            var safeArea = Screen.safeArea;
            _lastSafeArea = safeArea;
            PrintSafeArea(safeArea);
            
            
            _canvasScaler = GetComponent<CanvasScaler>();
            if (_canvasScaler != null)
            {
                _canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                _canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            
                // 根据屏幕比例决定匹配策略
                var screenRatio = (float)Screen.width / Screen.height;
                var designRatio = _canvasScaler.referenceResolution.x / _canvasScaler.referenceResolution.y;
                _canvasScaler.matchWidthOrHeight = screenRatio > designRatio ? 1 : 0;
            }
        
            ApplySafeArea();
        } 
    
        private void ApplySafeArea()
        {
            var safeArea = Screen.safeArea;
            var rectTransform = transform.Find("SafeArea").GetComponentInChildren<RectTransform>();
            Debug.LogWarning($"safeArea: ${Screen.safeArea} screen area: {Screen.width}, {Screen.height}");
            
            // rectTransform.offsetMin = new Vector2(safeArea.xMin, Screen.height - safeArea.yMax);
            // rectTransform.offsetMax = new Vector2(safeArea.xMax - Screen.width, -safeArea.yMin);
            
            rectTransform.anchorMin = new Vector2(safeArea.xMin / Screen.width, safeArea.yMin / Screen.height);
            rectTransform.anchorMax = new Vector2(safeArea.xMax / Screen.width, safeArea.yMax / Screen.height);
            // LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            Debug.LogWarning($"ApplySafeArea: {rectTransform.anchorMin}, ${rectTransform.anchorMax}");
            Debug.LogWarning($"ApplySafeArea: {rectTransform.offsetMin}, ${rectTransform.offsetMax}");
        }
        
        
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

        private static void PrintSafeArea(Rect rect)
        {
            var safeArea = rect;
            Debug.LogWarning("Safe Area: " + safeArea);
            Debug.LogWarning("Screen Size: " + Screen.width + "x" + Screen.height);
            Debug.LogWarning($"position: {safeArea.position}, center: {safeArea.center}, minXY: {safeArea.xMin}, {safeArea.xMax}," +
                             $" {safeArea.yMin}, {safeArea.yMax},  min: {safeArea.min}, {safeArea.max} size: {safeArea.size}");
        }

        private void Update()
        {
            if (_lastSafeArea == Screen.safeArea) return;
            _lastSafeArea = Screen.safeArea;
            ApplySafeArea();
        }
        
        private void OnGUI()
        {
            #if UNITY_EDITOR
            GUI.depth = 0;
            // 转换 Screen.safeArea 到 GUI 坐标系
            var guiSafeArea = new Rect(
                Screen.safeArea.x,
                Screen.height - Screen.safeArea.yMax,
                Screen.safeArea.width,
                Screen.safeArea.height
            );
            GUI.Box(guiSafeArea, "SafeArea", _safeAreaStyle);
            #endif
        }
    }
}