using System;
using System.Text;
using GameBase;
using GameFramework;
using UnityEngine;

namespace GameConfig
{
    public class ConfigManager : Singleton<ConfigManager>, CfgDefine
    {

        private readonly string _version;
        
        public string Version
        {
            get;
            private set;
        }
        public ConfigManager()
        {
            
            var textAsset = GameModule.Resource.LoadAsset<TextAsset>("version.txt");
            Version = Encoding.UTF8.GetString(textAsset.bytes);
        }

        void CfgDefine.InitLoad(string tableName)
        {
            var textAsset = GameModule.Resource.LoadAsset<TextAsset>(tableName + ".json");
            if (textAsset == null)
            {
                throw new GameFrameworkException($"Load table {tableName}.json failed.");
            }
            var data = Encoding.UTF8.GetString(textAsset.bytes);
            var loadSize = ((CfgDefine)this).ReloadCfg(tableName, data);
            Console.WriteLine($"load table {tableName}, size: {loadSize}");
        }
    }
}