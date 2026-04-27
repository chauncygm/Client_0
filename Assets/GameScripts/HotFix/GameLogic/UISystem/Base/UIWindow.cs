using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;
using Object = UnityEngine.Object;

namespace GameLogic
{
    public abstract class UIWindow : UIBase
    {
        #region Propreties

        private Action<UIWindow> _prepareCallback;

        private bool _isCreate;

        private GameObject _panel;

        private Canvas _canvas;

        private Canvas[] _childCanvas;

        private GraphicRaycaster _raycaster;

        private GraphicRaycaster[] _childRaycaster;

        public override UIType Type => UIType.Window;

        /// <summary>
        /// 窗口位置组件。
        /// </summary>
        public override Transform Transform => _panel.transform;
        
        /// <summary>
        /// 窗口矩阵位置组件。
        /// </summary>
        public override RectTransform RectTransform => _panel.transform as RectTransform;

        /// <summary>
        /// 窗口的实例资源对象。
        /// </summary>
        public override GameObject GameObject => _panel;

        /// <summary>
        /// 窗口名称。
        /// </summary>
        public string WindowName { private set; get; }

        /// <summary>
        /// 窗口层级。
        /// </summary>
        public int WindowLayer { private set; get; }

        /// <summary>
        /// 资源定位地址。
        /// </summary>
        public string AssetName { private set; get; }

        /// <summary>
        /// 是否为全屏窗口。
        /// </summary>
        public virtual bool FullScreen { private set; get; }

        /// <summary>
        /// 是内部资源无需AB加载。
        /// </summary>
        public bool FromResources { private get; set; }
        
        /// <summary>
        /// 隐藏窗口关闭时间。
        /// </summary>
        public int HideTimeToClose { get; set; }
        
        public int HideTimerId { get; set; }

        /// <summary>
        /// 自定义数据。
        /// </summary>
        protected object UserData => UserDatas is { Length: >= 1 } ? UserDatas[0] : null;

        /// <summary>
        /// 窗口深度值。
        /// </summary>
        public int Depth
        {
            get => _canvas != null ? _canvas.sortingOrder : 0;

            set
            {
                if (!_canvas) return;
                
                if (_canvas.sortingOrder == value)
                {
                    return;
                }

                // 设置父类
                _canvas.sortingOrder = value;

                // 设置子类
                var depth = value;
                foreach (var canvas in _childCanvas)
                {
                    if (canvas == _canvas) continue;
                    depth += 5; //注意递增值
                    canvas.sortingOrder = depth;
                }

                // 虚函数
                if (_isCreate)
                {
                    OnSortDepth(value);
                }
            }
        }

        /// <summary>
        /// 窗口可见性
        /// </summary>
        public bool Visible
        {
            get
            {
                if (_canvas)
                {
                    return _canvas.gameObject.layer == UISystem.WindowShowLayer;
                }
                return false;
            }

            set
            {
                if (!_canvas) return;
                
                var setLayer = value ? UISystem.WindowShowLayer : UISystem.WindowHideLayer;
                if (_canvas.gameObject.layer == setLayer)
                    return;

                // 显示设置
                _canvas.gameObject.layer = setLayer;
                foreach (var canvas in _childCanvas)
                {
                    canvas.gameObject.layer = setLayer;
                }

                // 交互设置
                Interactable = value;

                // 虚函数
                if (_isCreate)
                {
                    OnSetVisible(value);
                }
            }
        }

        /// <summary>
        /// 窗口交互性
        /// </summary>
        private bool Interactable
        {
            get => _raycaster != null && _raycaster.enabled;

            set
            {
                if (!_raycaster) return;
                
                _raycaster.enabled = value;
                foreach (var raycaster in _childRaycaster)
                {
                    raycaster.enabled = value;
                }
            }
        }

        /// <summary>
        /// 是否加载完毕。
        /// </summary>
        internal bool IsLoadDone;

        #endregion

        public void Init(string name, int layer, bool fullScreen, string assetName, bool fromResources, int hideTimeToClose)
        {
            WindowName = name;
            WindowLayer = layer;
            FullScreen = fullScreen;
            AssetName = assetName;
            FromResources = fromResources;
            HideTimeToClose = hideTimeToClose;
        }

        internal void TryInvoke(Action<UIWindow> prepareCallback, object[] userDatas)
        {
            UserDatas = userDatas;
            if (IsPrepare)
            {
                prepareCallback?.Invoke(this);
            }
            else
            {
                _prepareCallback = prepareCallback;
            }
            CancelHideToCloseTimer();
        }

        internal async UniTaskVoid InternalLoad(string location, Action<UIWindow> prepareCallback, bool isAsync, object[] userDatas)
        {
            _prepareCallback = prepareCallback;
            UserDatas = userDatas;
            if (!FromResources)
            {
                if (isAsync)
                {
                    var uiInstance = await GameModule.Resource.LoadGameObjectAsync(location, parent: UISystem.Instance.UICanvasTransform);
                    Handle_Completed(uiInstance);
                }
                else
                {
                    var uiInstance = GameModule.Resource.LoadGameObject(location, parent: UISystem.Instance.UICanvasTransform);
                    Handle_Completed(uiInstance);
                }
            }
            else
            {
                var panel = Object.Instantiate(Resources.Load<GameObject>(location), UISystem.Instance.UICanvasTransform);
                Handle_Completed(panel);
            }
        }

        internal void InternalCreate()
        {
            if (_isCreate) return;
            
            _isCreate = true;
            ScriptGenerator();
            BindMemberProperty();
            RegisterEvent();
            OnCreate();
        }

        internal void InternalRefresh()
        {
            OnRefresh();
        }

        internal bool InternalUpdate()
        {
            if (!IsPrepare || !Visible)
            {
                return false;
            }

            List<UIWidget> listNextUpdateChild = null;
            if (ListChild is { Count: > 0 })
            {
                listNextUpdateChild = ListUpdateChild;
                var updateListValid = UpdateListValid;
                List<UIWidget> listChild;
                if (!updateListValid)
                {
                    if (listNextUpdateChild == null)
                    {
                        listNextUpdateChild = new List<UIWidget>();
                        ListUpdateChild = listNextUpdateChild;
                    }
                    else
                    {
                        listNextUpdateChild.Clear();
                    }

                    listChild = ListChild;
                }
                else
                {
                    listChild = listNextUpdateChild;
                }

                foreach (var uiWidget in listChild.Where(uiWidget => uiWidget != null))
                {
                    TProfiler.BeginSample(uiWidget.name);
                    var needValid = uiWidget.InternalUpdate();
                    TProfiler.EndSample();

                    if (!updateListValid && needValid)
                    {
                        listNextUpdateChild.Add(uiWidget);
                    }
                }

                if (!updateListValid)
                {
                    UpdateListValid = true;
                }
            }

            TProfiler.BeginSample("OnUpdate");

            bool needUpdate;
            if (listNextUpdateChild == null || listNextUpdateChild.Count <= 0)
            {
                HasOverrideUpdate = true;
                OnUpdate();
                needUpdate = HasOverrideUpdate;
            }
            else
            {
                OnUpdate();
                needUpdate = true;
            }

            TProfiler.EndSample();

            return needUpdate;
        }

        internal void InternalDestroy()
        {
            _isCreate = false;

            RemoveAllUIEvent();

            foreach (var uiChild in ListChild)
            {
                uiChild.CallDestroy();
                uiChild.OnDestroyWidget();
            }

            // 注销回调函数
            _prepareCallback = null;

            OnDestroy();

            // 销毁面板对象
            if (_panel != null)
            {
                Object.Destroy(_panel);
                _panel = null;
            }
            CancelHideToCloseTimer();
        }

        /// <summary>
        /// 处理资源加载完成回调。
        /// </summary>
        /// <param name="panel">面板资源实例。</param>
        private void Handle_Completed(GameObject panel)
        {
            if (!panel)
            {
                return;
            }

            IsLoadDone = true;
            
            panel.name = GetType().Name;
            _panel = panel;
            _panel.transform.localPosition = Vector3.zero;

            // 获取组件
            _canvas = _panel.GetComponent<Canvas>();
            if (!_canvas)
            {
                throw new Exception($"Not found {nameof(Canvas)} in panel {WindowName}");
            }

            _canvas.overrideSorting = true;
            _canvas.sortingOrder = 0;
            _canvas.sortingLayerName = "Default";

            // 获取组件
            _raycaster = _panel.GetComponent<GraphicRaycaster>();
            _childCanvas = _panel.GetComponentsInChildren<Canvas>(true);
            _childRaycaster = _panel.GetComponentsInChildren<GraphicRaycaster>(true);

            // 通知UI管理器
            IsPrepare = true;
            _prepareCallback?.Invoke(this);
        }

        protected void Close()
        {
            UISystem.Instance.CloseUI(GetType());
        }

        internal void CancelHideToCloseTimer()
        {
            if (HideTimerId <= 0) return;
            
            GameModule.Timer.CancelTimer(HideTimerId);
            HideTimerId = 0;
        }
    }
}