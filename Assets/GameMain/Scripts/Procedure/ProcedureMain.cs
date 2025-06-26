using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Scripts.Procedure
{
    public class ProcedureMain : BaseProcedure
    {

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            var variable = (VarInt32)procedureOwner.GetData("LoginUIForm");
            if (variable != null)
            {
                Base.GameEntry.UI.CloseUIForm(variable);
            }

            Base.GameEntry.UI.OpenUIForm("GameMain/Prefab/UI/Main.prefab", "Normal");
            Debug.Log("Main start");
        }
    }
}