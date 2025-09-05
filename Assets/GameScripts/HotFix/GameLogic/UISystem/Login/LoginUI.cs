using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace GameLogic
{
    [Window(UILayer.UI, "Assets/Res/Prefab/UI/Login/LoginUI", true)]
    class LoginUI : UIWindow
    {
        #region 脚本工具生成的代码
        private Button m_btnServer;
        private Transform m_tfServerList;
        private TMP_InputField m_inputAccount;
        private Button m_btnLogin;
        private ServerListWidget m_serverListWidget;
        
        protected override void ScriptGenerator()
        {
            m_btnServer = FindChildComponent<Button>("m_btnServer");
            m_tfServerList = FindChildComponent<Transform>("m_tfServerList");
            m_inputAccount = FindChildComponent<TMP_InputField>("Account Area/m_inputAccount");
            m_btnLogin = FindChildComponent<Button>("m_btnLogin");
            m_btnServer.onClick.AddListener(UniTask.UnityAction(OnClickServerBtn));
            m_btnLogin.onClick.AddListener(UniTask.UnityAction(OnClickLoginBtn));
            m_serverListWidget = CreateWidgetByType<ServerListWidget>(m_tfServerList);
        }
        #endregion

        #region 事件
        private async UniTaskVoid OnClickServerBtn()
        {
            m_tfServerList.gameObject.SetActive(true);
            await UniTask.Yield();
        }
        
        private async UniTaskVoid OnClickLoginBtn()
        {
            var text = m_inputAccount.text;
            if (long.TryParse(text, out var uid))
            {
                GameEvent.EventMgr.GetInterface<ILoginUI>().OnRoleLogin(uid);
            }
            await UniTask.Yield();
        }
        #endregion

        protected override void OnCreate()
        {
            base.OnCreate();
            m_tfServerList.gameObject.SetActive(false);
            AddUIEvent<string>(ILoginUI_Event.OnSelectServer, OnSelectServer);
        }

        private void OnSelectServer(string serverName)
        {
            m_btnServer.GetComponent<TMP_Text>().text = serverName;
            m_tfServerList.gameObject.SetActive(false);
        }
    }
}


