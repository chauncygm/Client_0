using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using UnityGameFramework.Runtime;

namespace GameConfig
{
    public abstract class ConfigBase<T>
    {
        private static IDictionary<int, T> _dataMap = new Dictionary<int, T>();
    
        public int Id { get; protected set; }
    
        public static T Get(int id)
        {
            return _dataMap[id];
        }
    
        public static IDictionary<int, T> GetAll()
        {
            return _dataMap;
        }
    
        public static int Reload(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return 0;
            }

            try
            {
                _dataMap = JsonConvert.DeserializeObject<Dictionary<int, T>>(data) ?? new Dictionary<int, T>();
                _dataMap = new ReadOnlyDictionary<int, T>(_dataMap);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return _dataMap.Count;
        }
    }
}