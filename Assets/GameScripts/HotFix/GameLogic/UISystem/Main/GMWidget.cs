using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.UI, "Assets/Res/Prefab/UI/Main/GMWidget")]
    public class GMWidget : UIWidget
    {
        #region 脚本工具生成的代码
        private Button m_btnGM;
        private GameObject m_goGMPanel;
        private TMP_InputField m_inputCmd1;
        private Button m_btnSendGm1;
        private TMP_InputField m_inputCmd2;
        private Button m_btnSendGm2;
        protected override void ScriptGenerator()
        {
            m_btnGM = FindChildComponent<Button>("m_btnGM");
            m_goGMPanel = FindChild("m_goGMPanel").gameObject;
            m_inputCmd1 = FindChildComponent<TMP_InputField>("m_goGMPanel/Area1/m_inputCmd1");
            m_btnSendGm1 = FindChildComponent<Button>("m_goGMPanel/Area1/m_btnSendGm1");
            m_inputCmd2 = FindChildComponent<TMP_InputField>("m_goGMPanel/Area2/m_inputCmd2");
            m_btnSendGm2 = FindChildComponent<Button>("m_goGMPanel/Area2/m_btnSendGm2");
            m_btnGM.onClick.AddListener(UniTask.UnityAction(OnClickGMBtn));
            m_btnSendGm1.onClick.AddListener(UniTask.UnityAction(OnClickSendGm1Btn));
            m_btnSendGm2.onClick.AddListener(UniTask.UnityAction(OnClickSendGm2Btn));
        }
        #endregion

        protected override void OnCreate()
        {
            base.OnCreate();
            m_goGMPanel.SetActive(false);
        }

        #region 事件
        private async UniTaskVoid OnClickGMBtn()
        {
            var show = !m_goGMPanel.activeSelf;
            m_goGMPanel.SetActive(show);
            await UniTask.Yield();
        }
        private async UniTaskVoid OnClickSendGm1Btn()
        {
            PlayerManager.SendGM("setLvExp", m_inputCmd1.text);
            m_inputCmd1.text = string.Empty;
            await UniTask.Yield();
        }
        private async UniTaskVoid OnClickSendGm2Btn()
        {
            PlayerManager.SendGM("addResource", m_inputCmd2.text);
            m_inputCmd2.text = string.Empty;
            await UniTask.Yield();
        }
        #endregion

    }

}


