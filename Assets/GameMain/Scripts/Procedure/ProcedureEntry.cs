using System.Collections;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;
using GameEntry = GameMain.Scripts.Base.GameEntry;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Scripts.Procedure {
    public class ProcedureEntry : BaseProcedure
    {

        private int _splashPanelSerialId;

        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
            GameEntry.Init();
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Debug.Log($"Project is running based on Game Framework {Version.GameFrameworkVersion}.");

            _splashPanelSerialId = GameEntry.UI.OpenUIForm("Assets/GameMain/Prefab/UI/SplashPanel.prefab", "Default");
            GameEntry.Event.Subscribe(CloseUIFormCompleteEventArgs.EventId, OnCloseUIFormCompleteEvent);
        }

        private void OnCloseUIFormCompleteEvent(object sender, GameEventArgs e)
        {
            var args = (CloseUIFormCompleteEventArgs)e;
            if (args.SerialId == _splashPanelSerialId)
            {
                ChangeProcedure<ProcedureLogin>();
            }
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            GameEntry.Event.Unsubscribe(CloseUIFormCompleteEventArgs.EventId, OnCloseUIFormCompleteEvent);
        }
    }
}
