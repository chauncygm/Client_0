using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureBase = GameMain.ProcedureBase;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameLogic
{
    public class ProcedureMain : ProcedureBase
    {
        

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            
            UISystem.Instance.ShowUI<MainUI>();
            Debug.Log("Main start");
            
            GameEvent.AddEventListener(IActorLogicEvent_Event.OnMainPlayerDisconnect, OnMainPlayerDisconnect);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            UISystem.Instance.CloseUI<LoginUI>();
            GameEvent.RemoveEventListener(IActorLogicEvent_Event.OnMainPlayerDisconnect, OnMainPlayerDisconnect);
        }
        
        

        private void OnMainPlayerDisconnect()
        {
            ChangeProcedure<ProcedureLogin>();
        }

    }
}