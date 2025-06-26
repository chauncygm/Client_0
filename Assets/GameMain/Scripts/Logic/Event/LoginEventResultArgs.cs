using GameFramework;
using GameFramework.Event;

namespace GameMain.Scripts.Logic.Event
{
    public class LoginEventResultArgs : GameEventArgs
    {
        /// <summary>
        /// 登录结果事件编号
        /// </summary>
        public const int EventId = (int)PlayerEvent.LoginResultEventID;

        public override int Id => EventId;

        public int Code { get; set; }

        public static LoginEventArgs Create()
        {
            return ReferencePool.Acquire<LoginEventArgs>();
        }

        public override void Clear()
        {
            Code = 0;
        }
    }
}