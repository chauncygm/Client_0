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

                    var parameterType = method.GetParameters()[0].ParameterType;
                    if (method.GetParameters().Length != 1 || !typeof(IMessage).IsAssignableFrom(parameterType))
                    {
                        Log.Error($"{method.Name} is not a valid message handler");
                        continue;
                    }

                    var handlerType = typeof(Action<>).MakeGenericType(parameterType);
                    var registerMethod = typeof(MessageDispatcher)
                        .GetMethod("RegisterHandler", new[] { typeof(Action<IMessage>) })
                        ?.MakeGenericMethod(parameterType);
                    Delegate handlerDelegate;
                    if (method.IsStatic)
                    {
                        handlerDelegate = Delegate.CreateDelegate(handlerType, method);
                    }
                    else
                    {
                        Debug.Assert(method.DeclaringType != null, "method.DeclaringType != null");
                        var instance = Activator.CreateInstance(method.DeclaringType);
                        handlerDelegate = Delegate.CreateDelegate(handlerType, instance, method);
                    }
                    registerMethod?.Invoke(null, new object[] { handlerDelegate });
                }
            }
        }

        public static void RegisterHandler<T>(Action<T> handler) where T : IMessage
        {
            var type = typeof(T);
            if (Handlers.ContainsKey(type))
            {
                Log.Error($"repeated register handler for {type}");
                return;
            }
            Handlers[type] = new MessageHandler<T>(handler);
        }

        private static void Dispatch(IMessage message)
        {
            var type = message.GetType();
            if (!Handlers.TryGetValue(type, out var handler))
            {
                UnityEngine.Debug.Log($"未注册的消息: {type}, {message}");
                return;
            }
            
            UnityEngine.Debug.Log($"收到消息{type}: {message}");
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