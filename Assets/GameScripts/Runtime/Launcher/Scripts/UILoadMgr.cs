using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityGameFramework.Runtime;
using Object = UnityEngine.Object;

namespace GameMain
{
    /// <summary>
    /// 热更界面加载管理器。
    /// </summary>
    public static class UILoadMgr
    {
        private const string UIRootPath = "UIRoot/UICanvas/SafeArea";
        
        private static Transform _uiRoot;
        private static readonly Dictionary<string, UIBase> UIMap = new();

        /// <summary>
        /// 初始化根节点。
        /// </summary>
        public static void Initialize()
        {
            _uiRoot = GameObject.Find(UIRootPath)?.transform;
            if (_uiRoot == null)
            {
                Log.Error("Failed to Find UIRoot. Please check the resource path");
                return;
            }
            UIMap.Add(UIDefine.UILoadUpdate, null);
            UIMap.Add(UIDefine.UILoadTip, null);
            UIMap.Add(UIDefine.UISplash, null);
        }

        /// <summary>
        /// 显示提示框，目前最多支持两个按钮
        /// </summary>
        /// <param name="desc">描述</param>
        /// <param name="showType">类型（MessageShowType）</param>
        /// <param name="onOk">点击事件</param>
        /// <param name="onCancel">取消事件</param>
        public static void ShowMessageBox(string desc, MessageShowType showType = MessageShowType.Tips,
            Action onOk = null, Action onCancel = null)
        {
            switch (showType)
            {
                case MessageShowType.Quit:
                    onOk = GameModule.QuitApplication;
                    onCancel = null;
                    break;
                case MessageShowType.RetryOrQuitTips:
                    onCancel = GameModule.QuitApplication;
                    break;
                case MessageShowType.OkOrCancel:
                case MessageShowType.Tips:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(showType), showType, null);
            }
            Show(UIDefine.UILoadTip, desc, showType, onOk, onCancel);
        }

        /// <summary>
        /// show ui
        /// </summary>
        /// <param name="uiInfo">对应的ui</param>
        /// <param name="param">参数</param>
        public static void Show(string uiInfo, params object[] param)
        {
            if (string.IsNullOrEmpty(uiInfo) || !UIMap.ContainsKey(uiInfo))
            {
                Log.Error($"not define ui: {uiInfo}");
                return;
            }

            if (UIMap[uiInfo] == null)
            {
                var obj = Resources.Load(uiInfo);
                if (obj != null)
                {
                    var ui = Object.Instantiate(obj) as GameObject;
                    if (ui)
                    {
                        ui.transform.SetParent(_uiRoot.transform);
                        ui.transform.localScale = Vector3.one;
                        ui.transform.localPosition = Vector3.zero;
                        var rect = ui.GetComponent<RectTransform>();
                        rect.sizeDelta = Vector2.zero;
                        var component = ui.GetComponent<UIBase>();
                        if (component)
                        {
                            UIMap[uiInfo] = component;
                        }
                    }
                }
            }

            if (!UIMap.TryGetValue(uiInfo, out var uiBase)) return;
            uiBase.gameObject.SetActive(true);
            uiBase.OnEnter(param);
        }

        /// <summary>
        /// 隐藏ui对象
        /// </summary>
        /// <param name="uiName">对应的ui</param>
        public static void Hide(string uiName)
        {
            if (!UIMap.TryGetValue(uiName, out var value)) return;
            value.gameObject.SetActive(false);
            Object.Destroy(UIMap[uiName].gameObject);
            UIMap.Remove(uiName);
        }

        /// <summary>
        /// 隐藏所有热更相关UI。
        /// </summary>
        public static void HideAll()
        {
            foreach (var item in UIMap.Where(item => item.Value && item.Value.gameObject))
            {
                Object.Destroy(item.Value.gameObject);
            }
            UIMap.Clear();
        }
    }
}