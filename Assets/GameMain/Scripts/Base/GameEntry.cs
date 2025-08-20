using GameFramework.Procedure;
using GameMain.Scripts.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain.Scripts.Base
{
    public class GameEntry : MonoBehaviour
    {

        /// <summary>
        /// 获取事件组件。
        /// </summary>
        public static EventComponent Event
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取网络组件。
        /// </summary>
        public static NetworkComponent Network
        {
            get;
            private set;
        }

    }
}