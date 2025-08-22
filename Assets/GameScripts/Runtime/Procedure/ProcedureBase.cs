using GameFramework;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityGameFramework.Runtime;

namespace GameMain
{
    public abstract class ProcedureBase : GameFramework.Procedure.ProcedureBase
    {
        /// <summary>
        /// 获取流程是否使用原生对话框
        /// 在一些特殊的流程（如游戏逻辑对话框资源更新完成前的流程）中，可以考虑调用原生对话框进行消息提示行为
        /// </summary>
        public virtual bool UseNativeDialog => false;

        protected IFsm<IProcedureManager> ProcedureOwner;

        protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnInit(procedureOwner);
            ProcedureOwner = procedureOwner;
        }

        public void ChangeProcedure<T>() where T : ProcedureBase
        {
            ChangeState<T>(ProcedureOwner);
        }

        protected void SetData<T>(string name, T data) where T : Variable
        {
            ProcedureOwner.SetData(name, data);
        }
    }
}
