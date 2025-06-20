using System;
using Google.Protobuf;

namespace GameMain.Scripts.Net
{
    
    public abstract class MessageHandlerBase
    {
        public abstract void Handle(IMessage message);
    }
    
    public class MessageHandler<T> : MessageHandlerBase where T : IMessage
    {
        private readonly Action<T> _handler;

        public MessageHandler(Action<T> handler)
        {
            _handler = handler;
        }

        public override void Handle(IMessage message)
        {
            if (message is T tMessage)
            {
                _handler?.Invoke(tMessage);
            }
        }
    }
}