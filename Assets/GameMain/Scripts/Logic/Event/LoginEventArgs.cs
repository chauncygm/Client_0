using GameFramework;
using GameFramework.Event;

namespace GameMain.Scripts.Logic.Event
{
    public class LoginEventArgs : GameEventArgs
    {
        /// <summary>
        /// 登录事件编号
        /// </summary>
        public const int EventId = (int)PlayerEvent.LoginEventID;

        public override int Id => EventId;
        public long Uid {get; private set;}

        public static LoginEventArgs Create(long uid)
        {
            var e = ReferencePool.Acquire<LoginEventArgs>();
            e.Uid = uid;
            return e;
        }

        public override void Clear()
        {
            Uid = 0;
        }
    }
}