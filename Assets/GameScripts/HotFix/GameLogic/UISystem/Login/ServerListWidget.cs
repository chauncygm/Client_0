using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace GameLogic
{
    [Window(UILayer.UI, "Assets/Res/Prefab/UI/Login/ServerListWidget", true)]
    class ServerListWidget : UIWidget
    {
       
        #region 脚本工具生成的代码
        private ScrollRect m_scrollArea;
        protected override void ScriptGenerator()
        {
            m_scrollArea = FindChildComponent<ScrollRect>("m_scrollArea");
        }
        #endregion

        protected override void OnCreate()
        {
            base.OnCreate();
            var serverChannelInfos = SettingsUtils.FrameworkGlobalSettings.ServerChannelInfos;
            var serverChannelInfo = serverChannelInfos
                .Find(x => x.ChannelName == SettingsUtils.FrameworkGlobalSettings.CurUseServerChannel);
            
            var goInst = GameModule.Resource.LoadGameObject("Assets/Res/Prefab/UI/Login/ServerInfo");
            for (var i = serverChannelInfo.ServerIpAndPorts.Count - 1; i >= 0; i--)
            {
                var serverIpAndPort = serverChannelInfo.ServerIpAndPorts[i];
                var serverInfo = i == 0 ? goInst : Object.Instantiate(goInst, m_scrollArea.content);
                serverInfo.GetComponentInChildren<TMP_Text>().text = serverIpAndPort.ServerName;
                serverInfo.GetComponent<Button>().onClick.AddListener(() => OnSelectServer(serverIpAndPort.ServerName));
            }
            
        }

        private void OnSelectServer(string serverName)
        {
            var serverChannelInfo = SettingsUtils.FrameworkGlobalSettings.ServerChannelInfos
                .Find(x => x.ChannelName == SettingsUtils.FrameworkGlobalSettings.CurUseServerChannel);
            serverChannelInfo.CurUseServerName = serverName;
            
            GameEvent.EventMgr.GetInterface<ILoginUI>().OnSelectServer(serverName);
        }
    }
}

