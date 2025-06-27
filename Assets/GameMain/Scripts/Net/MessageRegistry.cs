using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Google.Protobuf;
using ICSharpCode.SharpZipLib.Checksum;

namespace GameMain.Scripts.Net
{
    public class MessageRegistry
    {
        private readonly Dictionary<int, Type> _mProtocolIdToType = new();
        
        /// <summary>
        /// 注册协议
        /// </summary>
        /// <param name="protocolNamespace">协议所在的命名空间</param>
        public void RegisterProto(string protocolNamespace)
        {
            // 获取当前程序集
            var assembly = Assembly.GetExecutingAssembly();

            // 扫描指定命名空间下的所有类型
            var protocolTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IMessage).IsAssignableFrom(t) && t.Namespace == protocolNamespace);

            foreach (var type in protocolTypes)
            {
                // 简单处理，使用类型名crc32码作为协议号
                var protocolHash = GetProtoEnum(type.Name);
                _mProtocolIdToType[protocolHash] = type;
            }
            
        }

        /// <summary>
        /// 获取协议对应的解析器
        /// </summary>
        /// <param name="protocolId">协议号</param>
        /// <param name="parseFromMethod">解析方法</param>
        /// <param name="customErrorData">自定义的获取网络数据包解析错误消息</param>
        /// <returns>parser对象</returns>
        public object GetParser(int protocolId, out MethodInfo parseFromMethod, out String customErrorData)
        {
            customErrorData = null;
            parseFromMethod = null;
            
            // 从协议字典中获取类型
            var protocolType = _mProtocolIdToType[protocolId];
            if (protocolType == null)
            {
                customErrorData = $"未知的协议类型: {protocolId}";
                return null;
            }
            
            // 获取 Parser 属性（静态属性）
            var parserProperty = protocolType.GetProperty("Parser", BindingFlags.Public | BindingFlags.Static);
            if (parserProperty == null || !parserProperty.PropertyType.FullName!.StartsWith("Google.Protobuf.MessageParser"))
            {
                customErrorData = $"找不到有效的 Parser 属性: {protocolType.Name}";
                return null;
            }

            // 获取 ParseFrom 方法
            var parser = parserProperty.GetValue(null);
            parseFromMethod = parserProperty.PropertyType.GetMethod("ParseFrom", new[] { typeof(byte[]) });
            // ReSharper disable once InvertIf
            if (parseFromMethod == null)
            {
                customErrorData = $"找不到 ParseFrom 方法: {protocolType.Name}";
                return null;
            }

            return parser;
        }

        /// <summary>
        /// 获取简单协议名对应的协议号
        /// </summary>
        /// <param name="protoName">协议名，nameof(协议Type.Name)</param>
        /// <returns>协议号</returns>
        public static int GetProtoEnum(string protoName)
        {
            var crc32 = new Crc32();
            crc32.Update(Encoding.UTF8.GetBytes(protoName));
            return (int)crc32.Value;
        }

    }
    
}