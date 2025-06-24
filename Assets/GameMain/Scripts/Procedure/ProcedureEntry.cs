using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;
using GameEntry = GameMain.Scripts.Base.GameEntry;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Scripts.Procedure {
    public class ProcedureEntry : ProcedureBase
    {
        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
            GameEntry.Start();
        }

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
