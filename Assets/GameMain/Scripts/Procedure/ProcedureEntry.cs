using GameFramework;
using GameFramework.Event;
using GameMain.Scripts.Logic.Event;
using GameMain.Scripts.Logic.Player.Data;
using UnityEngine;
using UnityGameFramework.Runtime;
using GameEntry = GameMain.Scripts.Base.GameEntry;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Scripts.Procedure {
    public class ProcedureEntry : BaseProcedure
    {

        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
            GameEntry.Init();
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Debug.Log($"Project is running based on Game Framework {Version.GameFrameworkVersion}.");
            GameEntry.UI.OpenUIForm("Assets/GameMain/Prefabs/UI/LoginPanel.prefab", "Default");
            GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccessEvent);
        }

        private void OnOpenUIFormSuccessEvent(object sender, GameEventArgs e)
        {
            var args = (OpenUIFormSuccessEventArgs)e;
            var uiFormSerialId = args.UIForm.SerialId;
            Debug.Log($"open ui success, {uiFormSerialId}");
            SetData<VarInt32>("LoginUIForm", uiFormSerialId);
            GameEntry.Event.Subscribe(LoginEventArgs.EventId, OnLoginEventArgs);
        }
        
        private static void OnLoginEventArgs(object sender, GameEventArgs e)
        {
            var loginEventArgs = (LoginEventArgs)e;
            var player = Player.Self;
            if (player.Session.Channel != null)
            {
                Debug.Log("已登录，请勿重复登录");
                return;
            }

            player.Session.Uid = loginEventArgs.Uid;
            GameEntry.ChangeProcedure<ProcedureLogin>();
        }

    }
}
