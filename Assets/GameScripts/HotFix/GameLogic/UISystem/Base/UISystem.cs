using System;
using System.Collections.Generic;
using System.Linq;
using GameBase;
using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameLogic
{
    /// <summary>
    /// UI系统。
    /// </summary>
    public sealed class UISystem : BaseLogicSys<UISystem>
    {
        private bool _mEnableErrorLog = true;

        private readonly List<UIWindow> _stack = new(128);

        internal const int LayerDeep = 2000;
        internal const int WindowDeep = 100;
        internal const int WindowHideLayer = 2; // Ignore Raycast
        internal const int WindowShowLayer = 5; // UI

        /// <summary>
        /// UI根节点。
        /// </summary>
        public Transform UICanvasTransform { private set; get; }

        /// <summary>
        /// UI根节点Canvas。
        /// </summary>
        public Canvas UICanvas { private set; get; }

        /// <summary>
        /// UI根节点。
        /// </summary>
        public Camera UICamera { private set; get; }

        private ErrorLogger _errorLogger;
        
        public override bool OnInit()
        {
            base.OnInit();
            
            Log.Info("OnInit UISystem");
            
            UICanvasTransform = GameObject.Find("UIRoot/UICanvas/SafeArea").transform;

            UICanvas = UICanvasTransform.GetComponent<Canvas>();

            UICamera = GameObject.Find("UIRoot/UICamera").GetComponent<Camera>();

            UICanvasTransform.gameObject.layer = LayerMask.NameToLayer("UI");

            _mEnableErrorLog = GameModule.Debugger.ActiveWindowType switch
            {
                DebuggerActiveWindowType.AlwaysOpen => true,
                DebuggerActiveWindowType.OnlyOpenWhenDevelopment => Debug.isDebugBuild,
                DebuggerActiveWindowType.OnlyOpenInEditor => Application.isEditor,
                _ => false
            };

            if (_mEnableErrorLog)
            {
                _errorLogger = new ErrorLogger();
            }

            UIController.RegisterAllController();
            
            return true;
        }

        public override void OnDestroy()
        {
            if (_errorLogger != null)
            {
                _errorLogger.Dispose();
                _errorLogger = null;
            }

            CloseAll();
        }

        /// <summary>
        /// 获取所有层级下顶部的窗口。
        /// </summary>
        public UIWindow GetTopWindow()
        {
            if (_stack.Count == 0)
            {
                return null;
            }

            var topWindow = _stack[^1];
            return topWindow;
        }

        /// <summary>
        /// 获取指定层级下顶部的窗口。
        /// </summary>
        public UIWindow GetTopWindow(int layer)
        {
            UIWindow lastOne = null;
            foreach (var window in _stack.Where(window => window.WindowLayer == layer))
            {
                lastOne = window;
            }

            return lastOne;
        }
        
        /// <summary>
        /// 获取指定层级下顶部的窗口。
        /// </summary>
        public UIWindow GetTopWindow(UILayer layer)
        {
            return GetTopWindow((int)layer);
        }

        /// <summary>
        /// 是否有任意窗口正在加载。
        /// </summary>
        public bool IsAnyLoading()
        {
            return _stack.Any(window => !window.IsLoadDone);
        }

        /// <summary>
        /// 查询窗口是否存在。
        /// </summary>
        /// <typeparam name="T">界面类型。</typeparam>
        /// <returns>是否存在。</returns>
        public bool HasWindow<T>()
        {
            return HasWindow(typeof(T));
        }

        /// <summary>
        /// 查询窗口是否存在。
        /// </summary>
        /// <param name="type">界面类型。</param>
        /// <returns>是否存在。</returns>
        public bool HasWindow(Type type)
        {
            return IsContains(type.FullName);
        }

        /// <summary>
        /// 异步打开窗口。
        /// </summary>
        /// <param name="userDatas">用户自定义数据。</param>
        /// <returns>打开窗口操作句柄。</returns>
        public void ShowUIAsync<T>(params System.Object[] userDatas) where T : UIWindow
        {
            ShowUIImp(typeof(T), true, userDatas);
        }

        /// <summary>
        /// 异步打开窗口。
        /// </summary>
        /// <param name="type">界面类型。</param>
        /// <param name="userDatas">用户自定义数据。</param>
        /// <returns>打开窗口操作句柄。</returns>
        public void ShowUIAsync(Type type, params System.Object[] userDatas)
        {
            ShowUIImp(type, true, userDatas);
        }

        /// <summary>
        /// 同步打开窗口。
        /// </summary>
        /// <typeparam name="T">窗口类。</typeparam>
        /// <param name="userDatas">用户自定义数据。</param>
        /// <returns>打开窗口操作句柄。</returns>
        public void ShowUI<T>(params System.Object[] userDatas) where T : UIWindow
        {
            ShowUIImp(typeof(T), false, userDatas);
        }

        /// <summary>
        /// 同步打开窗口。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="userDatas"></param>
        /// <returns>打开窗口操作句柄。</returns>
        public void ShowUI(Type type, params System.Object[] userDatas)
        {
            ShowUIImp(type, false, userDatas);
        }

        private void ShowUIImp(Type type, bool isAsync, params System.Object[] userDatas)
        {
            var windowName = type.FullName;

            // 如果窗口已经存在
            if (IsContains(windowName))
            {
                var window = GetWindow(windowName);
                Pop(window); //弹出窗口
                Push(window); //重新压入
                window.TryInvoke(OnWindowPrepare, userDatas);
            }
            else
            {
                var window = CreateInstance(type);
                Push(window); //首次压入
                window.InternalLoad(window.AssetName, OnWindowPrepare, isAsync, userDatas).Forget();
            }
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public void CloseUI<T>() where T : UIWindow
        {
            CloseUI(typeof(T));
        }

        public void CloseUI(Type type)
        {
            var windowName = type.FullName;
            var window = GetWindow(windowName);
            if (window == null)
                return;

            window.InternalDestroy();
            Pop(window);
            OnSortWindowDepth(window.WindowLayer);
            OnSetWindowVisible();
        }
        
        public void HideUI<T>() where T : UIWindow
        {
            HideUI(typeof(T));
        }

        public void HideUI(Type type)
        {
            var windowName = type.FullName;
            var window = GetWindow(windowName);
            if (window == null)
            {
                return;
            }

            if (window.HideTimeToClose <= 0)
            {
                CloseUI(type);
                return;
            }
            
            window.Visible = false;
            window.HideTimerId = GameModule.Timer.AddOnceTimer(window.HideTimeToClose * 1000, () =>
            {
                CloseUI(type);
            });
        }

        /// <summary>
        /// 关闭所有窗口。
        /// </summary>
        public void CloseAll()
        {
            foreach (var window in _stack)
            {
                window.InternalDestroy();
            }

            _stack.Clear();
        }

        /// <summary>
        /// 关闭所有窗口除了。
        /// </summary>
        public void CloseAllWithOut(UIWindow withOut)
        {
            for (var i = _stack.Count - 1; i >= 0; i--)
            {
                var window = _stack[i];
                if (window == withOut)
                {
                    continue;
                }

                window.InternalDestroy();
                _stack.RemoveAt(i);
            }
        }

        /// <summary>
        /// 关闭所有窗口除了。
        /// </summary>
        public void CloseAllWithOut<T>() where T : UIWindow
        {
            for (var i = _stack.Count - 1; i >= 0; i--)
            {
                var window = _stack[i];
                if (window.GetType() == typeof(T))
                {
                    continue;
                }

                window.InternalDestroy();
                _stack.RemoveAt(i);
            }
        }

        private void OnWindowPrepare(UIWindow window)
        {
            OnSortWindowDepth(window.WindowLayer);
            window.InternalCreate();
            window.InternalRefresh();
            OnSetWindowVisible();
        }

        private void OnSortWindowDepth(int layer)
        {
            var depth = layer * LayerDeep;
            foreach (var window in _stack.Where(window => window.WindowLayer == layer))
            {
                window.Depth = depth;
                depth += WindowDeep;
            }
        }

        private void OnSetWindowVisible()
        {
            var isHideNext = false;
            for (var i = _stack.Count - 1; i >= 0; i--)
            {
                var window = _stack[i];
                if (!isHideNext)
                {
                    window.Visible = true;
                    if (window.IsPrepare && window.FullScreen)
                    {
                        isHideNext = true;
                    }
                }
                else
                {
                    window.Visible = false;
                }
            }
        }

        private UIWindow CreateInstance(Type type)
        {
            var window = Activator.CreateInstance(type) as UIWindow;
            var attribute = Attribute.GetCustomAttribute(type, typeof(WindowAttribute)) as WindowAttribute;

            if (window == null)
                throw new GameFrameworkException($"Window {type.FullName} create instance failed.");

            if (attribute != null)
            {
                var assetName = string.IsNullOrEmpty(attribute.Location) ? type.Name : attribute.Location;
                window.Init(type.FullName, attribute.WindowLayer, attribute.FullScreen, assetName, attribute.FromResources, attribute.HideTimeToClose);
            }
            else
            {
                window.Init(type.FullName, (int)UILayer.UI, fullScreen: window.FullScreen, assetName: type.Name, fromResources: false, hideTimeToClose: 10);
            }

            return window;
        }

        private UIWindow GetWindow(string windowName)
        {
            return _stack.FirstOrDefault(window => window.WindowName == windowName);
        }

        private bool IsContains(string windowName)
        {
            return _stack.Any(window => window.WindowName == windowName);
        }

        private void Push(UIWindow window)
        {
            // 如果已经存在
            if (IsContains(window.WindowName))
                throw new GameFrameworkException($"Window {window.WindowName} is exist.");

            // 获取插入到所属层级的位置
            var insertIndex = -1;
            for (var i = 0; i < _stack.Count; i++)
            {
                if (window.WindowLayer == _stack[i].WindowLayer)
                {
                    insertIndex = i + 1;
                }
            }

            // 如果没有所属层级，找到相邻层级
            if (insertIndex == -1)
            {
                for (var i = 0; i < _stack.Count; i++)
                {
                    if (window.WindowLayer > _stack[i].WindowLayer)
                    {
                        insertIndex = i + 1;
                    }
                }
            }

            // 如果是空栈或没有找到插入位置
            if (insertIndex == -1)
            {
                insertIndex = 0;
            }

            // 最后插入到堆栈
            _stack.Insert(insertIndex, window);
        }

        private void Pop(UIWindow window)
        {
            // 从堆栈里移除
            _stack.Remove(window);
        }

        public override void OnUpdate()
        {
            if (_stack == null)
            {
                return;
            }

            var count = _stack.Count;
            foreach (var window in _stack.TakeWhile(_ => _stack.Count == count))
            {
                window.InternalUpdate();
            }
        }
    }
}