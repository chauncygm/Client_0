using System;
using System.Collections.Generic;
using System.Reflection;

namespace GameBase
{
    public class CodeTypes
    {
        private static CodeTypes _instance;
        public static CodeTypes Instance => _instance ??= new CodeTypes();

        private readonly Dictionary<string, Type> _allTypes = new();
        private readonly UnOrderMultiMapSet<Type, Type> _types = new();
        
        public void Init(Assembly[] assemblies)
        {
            var addTypes = GetAssemblyTypes(assemblies);
            foreach (var (fullName, type) in addTypes)
            {
                _allTypes[fullName] = type;
                
                if (type.IsAbstract)
                {
                    continue;
                }
                
                // 记录所有的有BaseAttribute标记的的类型
                var objects = type.GetCustomAttributes(typeof(BaseAttribute), true);

                foreach (var o in objects)
                {
                    _types.Add(o.GetType(), type);
                }
            }
        }

        public HashSet<Type> GetTypes(Type systemAttributeType)
        {
            return !_types.ContainsKey(systemAttributeType) ? new HashSet<Type>() : _types[systemAttributeType];
        }

        public Dictionary<string, Type> GetTypes()
        {
            return _allTypes;
        }

        public Type GetType(string typeName)
        {
            return _allTypes[typeName];
        }

        private static Dictionary<string, Type> GetAssemblyTypes(params Assembly[] args)
        {
            var types = new Dictionary<string, Type>();
            foreach (var ass in args)
            {
                foreach (var type in ass.GetTypes())
                {
                    if (type.FullName != null)
                    {
                        types[type.FullName] = type;
                    }
                }
            }
            return types;
        }
    }
}