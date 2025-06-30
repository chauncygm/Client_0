using GameFramework;
using GameFramework.Event;
using GameMain.Scripts.Logic.Enum;

namespace GameMain.Scripts.Logic.Event
{
    public class LoginEventResultArgs : GameEventArgs
    {
        /// <summary>
        /// 登录结果事件编号
        /// </summary>
        public const int EventId = (int)PlayerEvent.LoginResultEventID;

        public override int Id => EventId;

        public LoginResult Result { get; private set; }

        public static LoginEventResultArgs Create(LoginResult result)
        {
            var loginEventArgs = ReferencePool.Acquire<LoginEventResultArgs>();
            loginEventArgs.Result = result;
            return loginEventArgs;
        }

        public override void Clear()
        {
            Result = 0;
        }
    }
}