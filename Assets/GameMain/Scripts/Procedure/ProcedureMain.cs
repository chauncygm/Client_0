using GameFramework.Event;
using GameMain.Scripts.Logic.Event;
using UnityEngine;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Scripts.Procedure
{
    public class ProcedureMain : BaseProcedure
    {

        private int mainPanelSerialId;
        
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            mainPanelSerialId = Base.GameEntry.UI.OpenUIForm("Assets/GameMain/Prefab/UI/MainPanel.prefab", "Default");
            Base.GameEntry.Event.Subscribe(PlayerInfoChangeEventArgs.EventId, OnPlayerInfoChangeEvent);
            Debug.Log("Main start");
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            Base.GameEntry.UI.CloseUIForm(mainPanelSerialId);
        }

        private static void OnPlayerInfoChangeEvent(object sender, GameEventArgs e)
        {
            Debug.Log("玩家信息改变");
        }
    }
}