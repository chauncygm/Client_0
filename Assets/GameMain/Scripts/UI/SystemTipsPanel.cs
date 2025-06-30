using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace GameMain.Scripts.UI
{
    public class SystemTipsPanel : UIFormLogic
    {

        [SerializeField] private TMP_Text tipsText;
        [SerializeField] private Button button;
        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            button.onClick.AddListener(OnClick);
            
            var tips = userData as string;
            tipsText.text = tips;
        }
        
        private void OnClick()
        {
            Base.GameEntry.UI.CloseUIForm(UIForm);
        }
    }
}