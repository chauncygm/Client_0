using GameFramework;
using GameFramework.Event;
using GameMain.Scripts.Logic.Event;
using GameMain.Scripts.Logic.Player.Data;
using UnityEngine;
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
        }

    }
}
