using GameMain.Scripts.Logic.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameEntry = GameMain.Scripts.Base.GameEntry;

namespace GameMain.Scripts.UI
{
    public class LoginPanel
    {
        
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button loginButton;

        protected void OnOpen(object userData)
        {
            loginButton.onClick.AddListener(OnLoginClick);
            Debug.Log("OnOpen LoginPanel");
        }

        protected void OnClose(bool isShutdown, object userData)
        {
            loginButton.onClick.RemoveListener(OnLoginClick);
        }


        private void OnLoginClick()
        {
            Debug.Log("点击登录事件");
            var accountUidString = inputField.text;
            if (int.TryParse(accountUidString, out var accountUid))
            {
                if (accountUid > 0)
                {
                    GameEntry.Event.Fire(this, LoginEventArgs.Create(accountUid));
                    return;
                }
            }
            Debug.Log($"无效的uid输入: {accountUidString}");
        }
    }
}