using GameFramework;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain.Scripts.Procedure
{
    public abstract class BaseProcedure : ProcedureBase
    {

        private IFsm<IProcedureManager> _procedureOwner;
    
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Log.Debug($"开始流程：OnEnter{GetType().Name}");
            Debug.Log($"开始流程：OnEnter{GetType().Name}");
        }

        protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnInit(procedureOwner);
            _procedureOwner = procedureOwner;
            Log.Debug($"流程初始化：OnInit{GetType()}");
        }

        protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            Log.Debug($"流程结束：OnLeave{this.GetType()}");
        }

        public void ChangeProcedure<T>() where T : BaseProcedure
        {
            ChangeState<T>(_procedureOwner);
        }

        public void SetData<T>(string name, T data) where T : Variable
        {
            _procedureOwner.SetData(name, data);
        }
    }
}