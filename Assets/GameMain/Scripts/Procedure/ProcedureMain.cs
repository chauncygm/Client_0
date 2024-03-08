using Example;
using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Scripts.Procedure
{
    public class ProcedureMain : ProcedureBase
    {

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            
            var welcomeMessage = Utility.Text.Format("Hellooo! This is an empty project based on Game Framework {0}.", Version.GameFrameworkVersion);
            Log.Info(welcomeMessage);
            Log.Warning(welcomeMessage);
            Log.Error(welcomeMessage);
            Debug.Log(welcomeMessage);
            ChangeState<ProcedureExample>(procedureOwner);
            
        }
    }
}