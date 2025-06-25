using GameMain.Scripts.Base;
using GameMain.Scripts.Logic.Event;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GameMain.Scripts.UIPanel
{
    public class LoginPanel : MonoBehaviour
    {
        
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button loginButton;

        private void Awake()
        {
            loginButton.onClick.AddListener(OnLogin);
        }


        private void OnLogin()
        {
            Debug.Log("开始登录");
            var accountUidString = inputField.text;
            var accountUid = int.Parse(accountUidString);
            GameEntry.Event.Fire(this, LoginEventArgs.Create(accountUid));
        }
    }
}