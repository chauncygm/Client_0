using GameFramework.Event;
using GameMain.Scripts.Logic.Event;
using GameMain.Scripts.Logic.Player.Data;
using TMPro;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain.Scripts.UI
{
    public class MainPanel : UIFormLogic
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text lvText;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            var playerData = Player.Self.Data;
            nameText.text = playerData.Name;
            lvText.text = $"Lv.{playerData.LevelExp.Level}";
            
            Base.GameEntry.Event.Subscribe(PlayerInfoChangeEventArgs.EventId, OnPlayerInfoChangeEvent);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            Base.GameEntry.Event.Unsubscribe(PlayerInfoChangeEventArgs.EventId, OnPlayerInfoChangeEvent);
        }

        private void OnPlayerInfoChangeEvent(object sender, GameEventArgs e)
        {
            var args = (PlayerInfoChangeEventArgs)e;
            var playerData = Player.Self.Data;
            nameText.text = playerData.Name;
            lvText.text = $"Lv.{playerData.LevelExp.Level}";
        }
    }
}