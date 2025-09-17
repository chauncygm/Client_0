using System.Text;
using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameConfig
{
    public class ConfigManager : GameBase.Singleton<ConfigManager>, CfgDefine
    {

        private bool _isInit;

        private string Version { get; set; }

        public void Initialization()
        {
            if (_isInit) return;
            var textAsset = GameModule.Resource.LoadAsset<TextAsset>("version");
            Version = Encoding.UTF8.GetString(textAsset.bytes);
            Log.Info($"CONFIG version: {Version}");
            ((CfgDefine)this).Init();
            _isInit = true;
        }

        void CfgDefine.InitLoad(string tableName)
        {
            var textAsset = GameModule.Resource.LoadAsset<TextAsset>(tableName);
            if (textAsset == null)
            {
                throw new GameFrameworkException($"Load table {tableName}.json failed.");
            }
            var loadSize = ((CfgDefine)this).ReloadCfg(tableName, textAsset.text);
            Log.Info($"load table {tableName}, size: {loadSize}");
        }
    }
}