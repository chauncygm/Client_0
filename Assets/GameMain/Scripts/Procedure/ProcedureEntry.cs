using GameFramework;
using UnityEngine;
using YooAsset;
using GameEntry = GameMain.Scripts.Base.GameEntry;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Scripts.Procedure {
    public class ProcedureEntry : BaseProcedure {

        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
            GameEntry.Init();
            YooAssets.Initialize();
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Debug.Log($"Project is running based on Game Framework {Version.GameFrameworkVersion}.");
            ChangeProcedure<ProcedureSplash>();
        }
    }
}
