using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain.Scripts.Procedure
{
    public abstract class ProcedureBase : GameFramework.Procedure.ProcedureBase
    {
    
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Log.Debug($"开始流程：OnEnter{this.GetType()}");
        }

        protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnInit(procedureOwner);
            Log.Debug($"流程初始化：OnInit{this.GetType()}");
        }

        protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            Log.Debug($"流程结束：OnLeave{this.GetType()}");
        }
    }
}