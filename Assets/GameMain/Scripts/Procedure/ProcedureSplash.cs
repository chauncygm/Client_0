using GameFramework.Event;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Scripts.Procedure
{
    /// <summary>
    /// 流程->闪屏
    /// </summary>
    public class ProcedureSplash : BaseProcedure
    {
        private int _splashPanelSerialId;
        

        private void OnCloseUIFormCompleteEvent(object sender, GameEventArgs e)
        {
            ChangeProcedure<ProcedureCheckVersion>();
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
        }
        
    }
}