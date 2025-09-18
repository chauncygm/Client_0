using System.Collections.Generic;
using UnityGameFramework.Runtime;

namespace GameMain
{
    public class UIDefine
    {
        public const string UILoadUpdate = "UILoadUpdate";
        public const string UILoadTip = "UILoadTip";
        public const string UISplash = "UISplash";

        /// <summary>
        /// 注册ui
        /// </summary>
        /// <param name="list"></param>
        public static void RegisterUI(Dictionary<string, string> list)
        {
            if (list == null)
            {
                Log.Error("[UIManager]list is null");
                return;
            }

            if (!list.ContainsKey(UILoadUpdate))
            {
                list.Add(UILoadUpdate, $"AssetLoad/{UILoadUpdate}");
            }

            if (!list.ContainsKey(UILoadTip))
            {
                list.Add(UILoadTip, $"AssetLoad/{UILoadTip}");
            }

            if (!list.ContainsKey(UISplash))
            {
                list.Add(UISplash, $"AssetLoad/{UISplash}");
            }
        }
    }
}