using UnityEngine;
using UnityEngine.UI;

public enum AdaptorType
{
    None,
    KeepDesign2D,
}
[RequireComponent(typeof(RectTransform))]
public class ScreenAdaptor : MonoBehaviour
{
    #region 配置参数
    [Header("适配模式")]
    public AdaptorType adaptorType = AdaptorType.None;
    
    [Header("设计UI尺寸 宽 * 高")]
    public Vector2 designUISize = new(1080, 1920);
    
    [Header("设计场景 宽 * 高 分辨率 = value * 200")]
    public Vector2 designSceneSize = new(5.4f, 9.6f);
    
    [Header("最大场景 宽 * 高 分辨率 = value * 200")]
    public Vector2 maxSceneSize = new(5.4f, 9.6f);
    
    [Header("宽屏基准宽高比例")]
    public float wideScreenSize = 1.67f;
    #endregion

    #region 运行时变量
    private Vector2 _screenSize;
    private float _screenRatio;
    private bool _isWideScreen;
    private bool _isLandscape;

    private GameObject _startBgObject;
    private RectTransform _canvasRect;
    private RectTransform _panelRect;
    private RectTransform _uiAdaptorMask;

    private GameObject _sceneMask;
    private Transform _mask1;
    private Transform _mask2;
    private Vector3 _mask1DefaultPos;
    private Vector3 _mask2DefaultPos;
    private Vector3 _mask1TargetPos;
    private Vector3 _mask2TargetPos;
    
    private Camera _mainCamera;
    private float _orthographicSize;
    private Vector4 _safeAreaBounds;

    public static ScreenAdaptor Instance { get; private set; }

    #endregion

    private void Awake()
    {
        Instance = this;
        InitializeComponents();
    }

    private void Start()
    {
        CalculateScreenProperties();
        ApplyUIAdaption();
    }

    private void Update()
    {
        AnimateMask(_mask1, ref _mask1TargetPos);
        AnimateMask(_mask2, ref _mask2TargetPos);
    }

    private void InitializeComponents()
    {
        _mainCamera = Camera.main;
        _startBgObject = GameObject.Find("StartBg");
        
        if (_mainCamera?.transform.Find("SceneMask") != null)
            _sceneMask = _mainCamera.transform.Find("SceneMask").gameObject;
        if (_sceneMask)
        {
            _mask1 = _sceneMask.transform.Find("mask1");
            _mask2 = _sceneMask.transform.Find("mask2");
        }
            
        if (transform.Find("UIAdaptorMask"))
            _uiAdaptorMask = transform.Find("UIAdaptorMask").GetComponent<RectTransform>();
            
        var leftBottom = Screen.safeArea.position;
        var rightTop = leftBottom + Screen.safeArea.size;
        _safeAreaBounds = new Vector4(leftBottom.x, leftBottom.y, rightTop.x, rightTop.y);
        ShowMask(false);
    }

    private void CalculateScreenProperties()
    {
        _screenSize = new Vector2(Screen.width, Screen.height);
        _screenRatio = _screenSize.x > _screenSize.y ? _screenSize.x / _screenSize.y : _screenSize.y / _screenSize.x;
        _isLandscape = designSceneSize.x > designSceneSize.y;
        var designRatio = _isLandscape ? designSceneSize.x / designSceneSize.y : designSceneSize.y / designSceneSize.x;
        _isWideScreen = _screenRatio > designRatio && _screenRatio > wideScreenSize;
        
        if (adaptorType == AdaptorType.None) return;
        AdaptSceneBasedOnScreenType();
    }
    
    private void AdaptSceneBasedOnScreenType()
    {
        Vector3 maskScale, mask1Pos, mask2Pos;
        
        if (_isLandscape)
        {
            if (!_isWideScreen)
            {
                // 横屏非宽屏：高度固定，左右裁切
                _orthographicSize = designSceneSize.y;
                maskScale = new Vector3(10, 50, 0);
                mask1Pos = new Vector3(-maxSceneSize.x - 5, 0, 1);
                mask2Pos = new Vector3(maxSceneSize.x + 5, 0, 1);
            }
            else
            {
                // 横屏宽屏：宽度固定，上下裁切
                _orthographicSize = designSceneSize.x / _screenRatio;
                maskScale = new Vector3(50, 10, 0);
                mask1Pos = new Vector3(0, designSceneSize.y + 5, 1);
                mask2Pos = new Vector3(0, -designSceneSize.y - 5, 1);
            }
        }
        else
        {
            if (!_isWideScreen)
            {
                // 竖屏非宽屏：宽度固定，上下裁切
                _orthographicSize = designSceneSize.x * _screenRatio;
                maskScale = new Vector3(50, 10, 0);
                mask1Pos = new Vector3(0, maxSceneSize.y + 5, 1);
                mask2Pos = new Vector3(0, -maxSceneSize.y - 5, 1);
            }
            else
            {
                // 竖屏宽屏：高度固定，左右裁切
                _orthographicSize = designSceneSize.y;
                maskScale = new Vector3(10, 50, 0);
                mask1Pos = new Vector3(-designSceneSize.x - 5, 0, 1);
                mask2Pos = new Vector3(designSceneSize.x + 5, 0, 1);
            }
        }
        
        if (_mainCamera)
            _mainCamera.orthographicSize = _orthographicSize;
            
        if (_mask1)
        {
            _mask1.localScale = maskScale;
            _mask1.localPosition = _mask1DefaultPos = mask1Pos;
        }
        
        if (_mask2)
        {
            _mask2.localScale = maskScale;
            _mask2.localPosition = _mask2DefaultPos = mask2Pos;
        }
    }
    
    private void ApplyUIAdaption()
    {
        var canvasScaler = GetComponentInParent<CanvasScaler>();
        if (canvasScaler)
            canvasScaler.referenceResolution = designUISize;
            
        _canvasRect = transform.parent?.GetComponent<RectTransform>();
        _panelRect = GetComponent<RectTransform>();
        if (_panelRect)
        {
            _panelRect.anchorMin = Vector2.zero;
            _panelRect.anchorMax = Vector2.one;
            _panelRect.localPosition = Vector3.zero;
        }
        
        if (adaptorType == AdaptorType.None)
        {
            if (canvasScaler)
                canvasScaler.matchWidthOrHeight = _isLandscape ? 
                    (_isWideScreen ? 0f : 1f) : 
                    (_isWideScreen ? 1f : 0f);
                    
            UpdateScreenArea(_safeAreaBounds);
            UpdateUIMask(0);
            return;
        }
        
        if (_mask1) _mask1.gameObject.SetActive(true);
        if (_mask2) _mask2.gameObject.SetActive(true);
        
        Canvas.ForceUpdateCanvases();
        ProcessUIAdaption();
    }

    private void ProcessUIAdaption()
    {
        float left, bottom, right, top;
        
        if (_isLandscape)
        {
            // 横屏适配逻辑
            if (!_isWideScreen)
            {
                // 横屏非宽屏：UI上下顶满，左右裁切
                var halfWidth = _orthographicSize * _screenRatio;
                var width = halfWidth * 2;
                left = (halfWidth - maxSceneSize.x) / width * _screenSize.x;
                bottom = 0;
                right = (halfWidth + maxSceneSize.x) / width * _screenSize.x;
                top = _screenSize.y;
                
                var safeLeft = Mathf.Max(left, _safeAreaBounds.x);
                var safeRight = Mathf.Min(right, _safeAreaBounds.z);
                UpdateScreenArea(new Vector4(safeLeft, bottom, safeRight, top));
                
                if (maxSceneSize.x > halfWidth)
                {
                    var offset = (maxSceneSize.x - halfWidth) / width * _screenSize.x;
                    UpdateUIMask(offset);
                }
                else
                {
                    UpdateUIMask(0);
                }
            }
            else
            {
                // 横屏宽屏：UI左右顶满，上下裁切
                var height = _orthographicSize * 2;
                left = 0;
                bottom = (_orthographicSize - designSceneSize.y) / height * _screenSize.y;
                right = _screenSize.x;
                top = (_orthographicSize + designSceneSize.y) / height * _screenSize.y;
                
                left = Mathf.Max(left, _safeAreaBounds.x);
                bottom = Mathf.Max(bottom, _safeAreaBounds.y);
                right = Mathf.Min(right, _safeAreaBounds.z);
                top = Mathf.Min(top, _safeAreaBounds.w);
                UpdateScreenArea(new Vector4(left, bottom, right, top));
            }
        }
        else
        {
            // 竖屏适配逻辑
            if (!_isWideScreen)
            {
                // 竖屏非宽屏：UI左右顶满，上下裁切
                var height = _orthographicSize * 2;
                left = 0;
                bottom = (_orthographicSize - maxSceneSize.y) / height * _screenSize.y;
                right = _screenSize.x;
                top = (_orthographicSize + maxSceneSize.y) / height * _screenSize.y;
                
                bottom = Mathf.Max(bottom, _safeAreaBounds.y);
                top = Mathf.Min(top, _safeAreaBounds.w);
                UpdateScreenArea(new Vector4(left, bottom, right, top));
                
                if (maxSceneSize.y > _orthographicSize)
                {
                    var offset = (maxSceneSize.y - _orthographicSize) / height * _screenSize.y;
                    UpdateUIMask(offset);
                }
                else
                {
                    UpdateUIMask(0);
                }
            }
            else
            {
                // 竖屏宽屏：UI上下顶满，左右裁切
                var halfWidth = _orthographicSize / _screenRatio;
                var width = halfWidth * 2;
                left = (halfWidth - designSceneSize.x) / width * _screenSize.x;
                bottom = 0;
                right = (halfWidth + designSceneSize.x) / width * _screenSize.x;
                top = _screenSize.y;
                
                left = Mathf.Max(left, _safeAreaBounds.x);
                bottom = Mathf.Max(bottom, _safeAreaBounds.y);
                right = Mathf.Min(right, _safeAreaBounds.z);
                top = Mathf.Min(top, _safeAreaBounds.w);
                UpdateScreenArea(new Vector4(left, bottom, right, top));
            }
        }
    }

    private void AnimateMask(Transform mask, ref Vector3 targetPos)
    {
        if (targetPos == Vector3.zero || !mask) return;
        mask.localPosition = Vector3.MoveTowards(mask.localPosition, targetPos, 2 * Time.deltaTime);
        if (Vector3.Distance(mask.localPosition, targetPos) <= 0.001f)
        {
            mask.localPosition = targetPos;
            targetPos = Vector3.zero;
        }
    }

    public void UpdateMask(float moveDelta)
    {
        if (_isWideScreen) return;
        
        if (_isLandscape)
        {
            moveDelta = moveDelta * _screenRatio / 2;
            _mask1TargetPos = new Vector3(_mask1DefaultPos.x - moveDelta, _mask1DefaultPos.y, _mask1DefaultPos.z);
            _mask2TargetPos = new Vector3(_mask2DefaultPos.x + moveDelta, _mask2DefaultPos.y, _mask2DefaultPos.z);
        }
        else
        {
            moveDelta = moveDelta / _screenRatio / 2;
            _mask1TargetPos = new Vector3(_mask1DefaultPos.x, _mask1DefaultPos.y + moveDelta, _mask1DefaultPos.z);
            _mask2TargetPos = new Vector3(_mask2DefaultPos.x, _mask2DefaultPos.y - moveDelta, _mask2DefaultPos.z);
        }
    }

    public void ResetMask()
    {
        if (_isWideScreen) return;
        _mask1TargetPos = _mask1DefaultPos;
        _mask2TargetPos = _mask2DefaultPos;
    }

    public void ShowStartBg(bool show)
    {
        if (_startBgObject != null)
            _startBgObject.SetActive(show);
    }

    private void ShowMask(bool show)
    {
        if (_sceneMask)
            _sceneMask.SetActive(show);
        if (_uiAdaptorMask)
            _uiAdaptorMask.gameObject.SetActive(show);
    }

    private void UpdateScreenArea(Vector4 safeArea)
    {
        if (!_canvasRect || !_panelRect) return;
            
        var widthRatio = _canvasRect.sizeDelta.x / _screenSize.x;
        var heightRatio = _canvasRect.sizeDelta.y / _screenSize.y;

        var offsetMin = new Vector2(safeArea.x * widthRatio, safeArea.y * heightRatio);
        var offsetMax = new Vector2((safeArea.z - _screenSize.x) * widthRatio, 
                                   (safeArea.w - _screenSize.y) * heightRatio);
        
        _panelRect.offsetMin = offsetMin;
        _panelRect.offsetMax = offsetMax;
    }

    private void UpdateUIMask(float offset)
    {
        if (!_uiAdaptorMask || !_canvasRect || !_panelRect) return;
            
        if (_isLandscape)
        {
            var widthRatio = _canvasRect.sizeDelta.x / _screenSize.x;
            offset *= widthRatio;
            _uiAdaptorMask.offsetMin = new Vector2(-offset - _panelRect.offsetMin.x, 0);
            _uiAdaptorMask.offsetMax = new Vector2(offset - _panelRect.offsetMax.x, 0);
        }
        else
        {
            var heightRatio = _canvasRect.sizeDelta.y / _screenSize.y;
            offset *= heightRatio;
            _uiAdaptorMask.offsetMin = new Vector2(0, offset - _panelRect.offsetMin.y);
            _uiAdaptorMask.offsetMax = new Vector2(0, -offset - _panelRect.offsetMax.y);
        }
    }
    
    public Vector2 GetScreenSize()
    {
        if (adaptorType == AdaptorType.None)
            return _screenSize;
            
        float width, height;
        
        if (_isWideScreen)
        {
            if (_isLandscape)
            {
                width = _screenRatio * _mainCamera.orthographicSize;
            }
            else
            {
                width = designSceneSize.x;
            }

            height = designSceneSize.y;
        }
        else
        {
            if (_isLandscape)
            {
                var screenWidth = _screenRatio * _mainCamera.orthographicSize;
                width = screenWidth > designSceneSize.x ? designSceneSize.x : screenWidth;
                height = _mainCamera.orthographicSize;
            }
            else
            {
                var screenWidth = _mainCamera.orthographicSize / _screenRatio;
                width = screenWidth > designSceneSize.x ? designSceneSize.x : screenWidth;
                height = _mainCamera.orthographicSize;
            } 
        }
        
        return new Vector2(width * 200, height * 200);
    }
}