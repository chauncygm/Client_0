using System;
using GameBase;

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

        void CfgDefine.InitLoad(string tableName)
        {
            var data = "";
            var loadSize = ((CfgDefine)this).ReloadCfg(tableName, data);
            Console.WriteLine($"load table {tableName}, size: {loadSize}");
        }
    }
}