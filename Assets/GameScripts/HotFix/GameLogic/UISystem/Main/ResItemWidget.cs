using System.Collections.Generic;
using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace GameLogic
{
    [Window(UILayer.UI, "Assets/Res/Prefab/UI/Main/ResItemWidget")]
    class ResItemWidget : UIWidget
    {
        private int _resourceId;
        private Image m_imgIcon1;
        private TMP_Text m_textNum1;
        private Image m_imgIcon2;
        private TMP_Text m_textNum2;
        private Image m_imgIcon3;
        private TMP_Text m_textNum3;
        
        protected override void ScriptGenerator()
        {
            m_imgIcon1 = FindChildComponent<Image>("Res1/m_imgIcon1");
            m_textNum1 = FindChildComponent<TMP_Text>("Res1/m_textNum1");
            m_imgIcon2 = FindChildComponent<Image>("Res2/m_imgIcon2");
            m_textNum2 = FindChildComponent<TMP_Text>("Res2/m_textNum2");
            m_imgIcon3 = FindChildComponent<Image>("Res3/m_imgIcon3");
            m_textNum3 = FindChildComponent<TMP_Text>("Res3/m_textNum3");
        }

        protected override void RegisterEvent()
        {
            base.RegisterEvent();
            GameEvent.AddEventListener<int, int>(IBagLogicEvent_Event.OnResourceChange, OnResourceChange);
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            var res = Player.Self.Data.Resources;
            m_textNum1.text = res.GetValueOrDefault(1, 0).ToString();
            m_textNum2.text = res.GetValueOrDefault(2, 0).ToString();
            m_textNum3.text = res.GetValueOrDefault(3, 0).ToString();
        }

        private void OnResourceChange(int resourceId, int num)
        {
            switch (resourceId)
            {
                case 1:
                    m_textNum1.text = num.ToString();
                    break;
                case 2:
                    m_textNum2.text = num.ToString();
                    break;
                case 3:
                    m_textNum3.text = num.ToString();
                    break;
            }
        }
    }
}
