using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Google.Protobuf;

namespace GameMain.Scripts.Net
{
    public class MessageRegistry
    {
        private readonly Dictionary<int, Type> _mProtocolIdToType = new();
        
        
        public void RegisterProto(string protocolNamespace)
        {
            // 获取当前程序集
            var assembly = Assembly.GetExecutingAssembly();

            // 扫描指定命名空间下的所有类型
            var protocolTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IMessage).IsAssignableFrom(t) && t.Namespace == protocolNamespace);

            foreach (var type in protocolTypes)
            {
                // 使用类型的全名计算哈希值作为协议号
                System.Diagnostics.Debug.Assert(type.FullName != null, "type.FullName != null");
                var protocolHash = type.FullName.GetHashCode();
                _mProtocolIdToType[protocolHash] = type;
            }
            
        }

        public Object getParser(int protocolId, out MethodInfo parseFromMethod, out String customErrorData)
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
            if (parserProperty == null || parserProperty.PropertyType.FullName != "Google.Protobuf.MessageParser")
            {
                customErrorData = $"找不到有效的 Parser 属性: {protocolType.Name}";
                return null;
            }

            // 获取 ParseFrom 方法
            var parser = parserProperty.GetValue(null);
            parseFromMethod = parserProperty.PropertyType.GetMethod("ParseFrom", new[] { typeof(byte[]) });
            if (parseFromMethod == null)
            {
                customErrorData = $"找不到 ParseFrom 方法: {protocolType.Name}";
                return null;
            }

            return parser;
        }

    }
    
}