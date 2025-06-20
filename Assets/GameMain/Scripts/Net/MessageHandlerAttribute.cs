using System;

namespace GameMain.Scripts.Net
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MessageHandlerAttribute : Attribute
    {
        public Type MessageType { get; }

        public MessageHandlerAttribute(Type messageType)
        {
            MessageType = messageType;
        }
    }
}