using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace GameMain.Scripts.UI
{
    public class SystemTipsPanel
    {

        [SerializeField] private TMP_Text tipsText;
        [SerializeField] private Button button;
        
        protected void OnOpen(object userData)
        {
            var tips = userData as string;
            tipsText.text = tips;
        }
        
    }
}