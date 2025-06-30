using GameFramework;
using GameFramework.Event;

namespace GameMain.Scripts.Logic.Event
{
    public class PlayerInfoChangeEventArgs : GameEventArgs
    {
        /// <summary>
        /// 登录事件编号
        /// </summary>
        public const int EventId = (int)PlayerEvent.PlayerBaseInfoChangeEventID;

        public override int Id => EventId;

        public static PlayerInfoChangeEventArgs Create()
        {
            var e = ReferencePool.Acquire<PlayerInfoChangeEventArgs>();
            return e;
        }

        public override void Clear()
        {
            
        }
    }
}