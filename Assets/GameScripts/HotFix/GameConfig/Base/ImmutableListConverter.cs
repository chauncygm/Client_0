using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace GameConfig
{
    public class ImmutableListConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IReadOnlyList<>) ||
                   objectType == typeof(ReadOnlyCollection<>) ||
                   (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(IReadOnlyList<>));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            // 获取泛型参数类型
            var elementType = objectType.GetGenericArguments()[0];
            // 创建对应的 List<> 类型
            var listType = typeof(List<>).MakeGenericType(elementType);

            // 反序列化为 List<T>
            if (serializer.Deserialize(reader, listType) is not IList list)
            {
                return Activator.CreateInstance(typeof(ReadOnlyCollection<>).MakeGenericType(elementType),
                    Activator.CreateInstance(listType));
            }

            // 创建 ReadOnlyCollection 并返回
            var readOnlyListType = typeof(ReadOnlyCollection<>).MakeGenericType(elementType);
            return Activator.CreateInstance(readOnlyListType, list);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}