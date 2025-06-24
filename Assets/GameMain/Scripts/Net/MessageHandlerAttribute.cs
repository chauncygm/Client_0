using System;

namespace GameMain.Scripts.Net
{
    /// <summary>
    /// 表明协议处理器特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MessageHandlerAttribute : Attribute
    {
    }
}