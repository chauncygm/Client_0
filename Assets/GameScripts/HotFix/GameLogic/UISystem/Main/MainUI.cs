using TMPro;

namespace GameLogic
{
    [Window(UILayer.UI, "Assets/Res/Prefab/UI/MainUI")]
    class MainUI : UIWindow
    {
        #region 脚本工具生成的代码
        private TMP_Text m_textName;
        private TMP_Text m_textLevel;
        protected override void ScriptGenerator()
        {
            m_textName = FindChildComponent<TMP_Text>("BaseInfo/m_textName");
            m_textLevel = FindChildComponent<TMP_Text>("BaseInfo/m_textLevel");
        }
        #endregion


        protected override void OnCreate()
        {
            base.OnCreate();
            var playerData = Player.Self.Data;
            m_textName.text = playerData.Name;
            m_textLevel.text = playerData.LevelExp.Level.ToString();
        }
    }
}