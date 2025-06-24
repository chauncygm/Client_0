using GameFramework;
using GameMain.Scripts.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureBase = GameFramework.Procedure.ProcedureBase;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Example {
    public class ProcedureExample : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            var welcomeMessage = Utility.Text.Format("Hello! This is an empty project based on Game Framework {0}.", Version.GameFrameworkVersion);
            Log.Info(welcomeMessage);
            Log.Warning(welcomeMessage);
            Log.Error(welcomeMessage);
            Debug.Log(welcomeMessage);
            ChangeState<ProcedureLogin>(procedureOwner);
        }
    }
}
