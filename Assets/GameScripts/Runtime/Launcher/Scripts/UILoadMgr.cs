using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain
{
    /// <summary>
    /// 热更界面加载管理器。
    /// </summary>
    public static class UILoadMgr
    {
        private static Transform _uiRoot;
        private static readonly Dictionary<string, string> UIList = new();
        private static readonly Dictionary<string, UIBase> UIMap = new();

        /// <summary>
        /// 初始化根节点。
        /// </summary>
        public static void Initialize()
        {
            _uiRoot = GameObject.Find("UIRoot/UICanvas/SafeArea")?.transform;
            if (_uiRoot == null)
            {
                Log.Error("Failed to Find UIRoot. Please check the resource path");
                return;
            }
            UIList.Add(UIDefine.UILoadUpdate, $"AssetLoad/{UIDefine.UILoadUpdate}");
            UIList.Add(UIDefine.UILoadTip, $"AssetLoad/{UIDefine.UILoadTip}");
            UIList.Add(UIDefine.UISplash, $"AssetLoad/{UIDefine.UISplash}");
        }

        /// <summary>
        /// show ui
        /// </summary>
        /// <param name="uiInfo">对应的ui</param>
        /// <param name="param">参数</param>
        public static void Show(string uiInfo, object param = null)
        {
            if (string.IsNullOrEmpty(uiInfo))
            {
                return;
            }

            if (!UIList.ContainsKey(uiInfo))
            {
                Log.Error($"not define ui:{uiInfo}");
                return;
            }

            GameObject ui = null;
            if (!UIMap.ContainsKey(uiInfo))
            {
                var obj = Resources.Load(UIList[uiInfo]);
                if (obj != null)
                {
                    ui = Object.Instantiate(obj) as GameObject;
                    if (ui)
                    {
                        ui.transform.SetParent(_uiRoot.transform);
                        ui.transform.localScale = Vector3.one;
                        ui.transform.localPosition = Vector3.zero;
                        var rect = ui.GetComponent<RectTransform>();
                        rect.sizeDelta = Vector2.zero;
                    }
                }

                var component = ui?.GetComponent<UIBase>();
                if (component)
                {
                    UIMap.Add(uiInfo, component);
                }
            }

            if (!UIMap.TryGetValue(uiInfo, out var uiBase)) return;
            uiBase.gameObject.SetActive(true);
            if (param == null) return;
            UIMap[uiInfo].OnEnter(param);
        }

        /// <summary>
        /// 隐藏ui对象
        /// </summary>
        /// <param name="uiName">对应的ui</param>
        public static void Hide(string uiName)
        {
            if (string.IsNullOrEmpty(uiName))
            {
                return;
            }

            if (!UIMap.TryGetValue(uiName, out var value)) return;
            value.gameObject.SetActive(false);
            Object.DestroyImmediate(UIMap[uiName].gameObject);
            UIMap.Remove(uiName);
        }

        /// <summary>
        /// 获取显示的ui对象
        /// </summary>
        /// <param name="ui"></param>
        /// <returns></returns>
        public static UIBase GetActiveUI(string ui)
        {
            return UIMap.GetValueOrDefault(ui);
        }

        /// <summary>
        /// 隐藏所有热更相关UI。
        /// </summary>
        public static void HideAll()
        {
            foreach (var item in UIMap)
            {
                if (item.Value && item.Value.gameObject)
                {
                    Object.Destroy(item.Value.gameObject);
                }
            }

            UIMap.Clear();
        }
    }
}