using GameFramework.Event;
using GameLogic;
using GameMain;
using UnityEngine;
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
        }
    }
}