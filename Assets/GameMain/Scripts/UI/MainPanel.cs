using GameFramework.Event;
using GameMain.Scripts.Logic.Event;
using GameMain.Scripts.Logic.Player.Data;
using TMPro;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain.Scripts.UI
{
    public class MainPanel
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text lvText;

        protected void OnOpen(object userData)
        {
            var playerData = Player.Self.Data;
            nameText.text = playerData.Name;
            lvText.text = $"Lv.{playerData.LevelExp.Level}";
            
            Base.GameEntry.Event.Subscribe(PlayerInfoChangeEventArgs.EventId, OnPlayerInfoChangeEvent);
        }

        protected void OnClose(bool isShutdown, object userData)
        {
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