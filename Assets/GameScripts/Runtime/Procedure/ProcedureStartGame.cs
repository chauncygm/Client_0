using GameFramework.Fsm;
using GameFramework.Procedure;

namespace GameMain
{
    public class ProcedureStartGame : ProcedureBase
    {

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            UILoadMgr.DestroyAll();
        }
        
    }
}