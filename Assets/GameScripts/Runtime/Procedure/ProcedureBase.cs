using GameFramework;
using GameFramework.Fsm;
using GameFramework.Procedure;

namespace GameMain
{
    public abstract class ProcedureBase : GameFramework.Procedure.ProcedureBase
    {

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
