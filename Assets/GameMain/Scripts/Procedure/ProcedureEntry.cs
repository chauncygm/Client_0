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
            var curLoginUIForm = GameEntry.UI.OpenUIForm("Assets/GameMain/Prefabs/UI/Login.prefab", "Normal");
            procedureOwner.SetData("LoginUIForm", (VarInt32)curLoginUIForm);
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
