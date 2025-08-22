using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace GameLogic
{
    [Window(UILayer.UI, "Assets/Res/Prefab/UI/LoginUI", true)]
    class LoginUI : UIWindow
    {
        #region 脚本工具生成的代码
        private TMP_InputField m_inputAccount;
        private Button m_btnLogin;
        
        protected override void ScriptGenerator()
        {
            m_inputAccount = FindChildComponent<TMP_InputField>("Account Area/m_inputAccount");
            m_btnLogin = FindChildComponent<Button>("m_btnLogin");
            m_btnLogin.onClick.AddListener(UniTask.UnityAction(OnClickLoginBtn));
        }
        #endregion

        #region 事件
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
        
        
        
    }
}