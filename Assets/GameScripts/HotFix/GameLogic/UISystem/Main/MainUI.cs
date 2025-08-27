using TMPro;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameLogic
{
    [Window(UILayer.UI, "Assets/Res/Prefab/UI/Main/MainUI")]
    class MainUI : UIWindow
    {
        #region 脚本工具生成的代码
        private TMP_Text m_textName;
        private TMP_Text m_textLevel;
        private GameObject m_itemResItem;
        private GameObject m_itemGM;
        protected override void ScriptGenerator()
        {
            m_textName = FindChildComponent<TMP_Text>("BaseInfo/m_textName");
            m_textLevel = FindChildComponent<TMP_Text>("BaseInfo/m_textLevel");
            m_itemResItem = FindChild("m_itemResItem").gameObject;
            m_itemGM = FindChild("m_itemGM").gameObject;
        }
        #endregion

        protected override void BindMemberProperty()
        {
            base.BindMemberProperty();
            CreateWidgetByType<ResItemWidget>(m_itemResItem.transform);
            CreateWidgetByType<GMWidget>(m_itemGM.transform);
        }

        protected override void RegisterEvent()
        {
            base.RegisterEvent();
            GameEvent.AddEventListener(IActorLogicEvent_Event.OnMainPlayerLevelChange, OnBaseInfoChange);
            GameEvent.AddEventListener(IActorLogicEvent_Event.OnMainPlayerNameChange, OnBaseInfoChange);
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            var playerData = Player.Self.Data;
            m_textName.text = playerData.Name;
            m_textLevel.text = $"LV.{playerData.LevelExp.Level.ToString()}";
        }
        
        void OnBaseInfoChange()
        {
            var playerData = Player.Self.Data;
            m_textName.text = playerData.Name;
            m_textLevel.text = $"LV.{playerData.LevelExp.Level.ToString()}";
        }
    }

}





