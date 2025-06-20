using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using GameFramework.Network;
using Google.Protobuf;
using UnityGameFramework.Runtime;

namespace GameMain.Scripts.Net
{
    public class MessageDispatcher : IPacketHandler
    {
        private static readonly Dictionary<Type, MessageHandlerBase> Handlers = new();

        public MessageDispatcher()
        {
            AutoScanAndRegisterHandlers();
        }

        private static void AutoScanAndRegisterHandlers()
        {
            // 获取当前程序集
            var assembly = Assembly.GetExecutingAssembly();
            foreach (var type in assembly.GetTypes())
            {
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                {
                    if (method.GetCustomAttributes(typeof(MessageHandlerAttribute), false)
                            .FirstOrDefault() is not MessageHandlerAttribute attribute)
                        continue;

                    if (method.GetParameters().Length != 1 || typeof(IMessage).IsAssignableFrom(method.GetParameters()[0].ParameterType))
                    {
                        Log.Error($"{method.Name} is not a valid message handler");
                        continue;
                    }

                    if (method.IsStatic)
                    {
                        // 静态方法：直接注册
                        var handlerType = typeof(Action<>).MakeGenericType(attribute.MessageType);
                        var handlerDelegate = Delegate.CreateDelegate(handlerType, method);

                        var registerMethod = typeof(MessageDispatcher)
                            .GetMethod("RegisterHandler", new[] { handlerType })
                            ?.MakeGenericMethod(attribute.MessageType);

                        registerMethod?.Invoke(null, new object[] { handlerDelegate });
                    }
                    else
                    {
                        // 实例方法：需要创建实例或从对象池中获取
                        Debug.Assert(method.DeclaringType != null, "method.DeclaringType != null");
                        var instance = Activator.CreateInstance(method.DeclaringType);
                        var handler = (Action<IMessage>)Delegate.CreateDelegate(typeof(Action<IMessage>), instance, method);

                        RegisterHandler(handler);
                    }
                }
            }
        }

        public static void RegisterHandler<T>(Action<T> handler) where T : IMessage
        {
            var type = typeof(T);
            if (!Handlers.ContainsKey(type))
            {
                Log.Error($"repeated register handler for {type}");
            }
            Handlers[type] = new MessageHandler<T>(handler);
        }

        private static void Dispatch(IMessage message)
        {
            var type = message.GetType();
            if (!Handlers.TryGetValue(type, out var handler)) return;
            handler.Handle(message);
        }

        public int Id => ProtoMessage.EventId;

        public void Handle(object sender, Packet packet)
        {
            if (packet is ProtoMessage protoMessage)
            {
                Dispatch(protoMessage.Data);
            }

        }
    }
}