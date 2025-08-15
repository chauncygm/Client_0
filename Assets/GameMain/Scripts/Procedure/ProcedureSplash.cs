using GameFramework.Event;
using UnityGameFramework.Runtime;
using GameEntry = GameMain.Scripts.Base.GameEntry;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Scripts.Procedure
{
    /// <summary>
    /// 流程->闪屏
    /// </summary>
    public class ProcedureSplash : BaseProcedure
    {
        private int _splashPanelSerialId;
        
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            _splashPanelSerialId = GameEntry.UI.OpenUIForm("Assets/GameMain/Prefab/UI/SplashPanel.prefab", "Default");
            GameEntry.Event.Subscribe(CloseUIFormCompleteEventArgs.EventId, OnCloseUIFormCompleteEvent);
        }

        private void OnCloseUIFormCompleteEvent(object sender, GameEventArgs e)
        {
            var args = (CloseUIFormCompleteEventArgs)e;
            if (args.SerialId == _splashPanelSerialId)
            {
                ChangeProcedure<ProcedureCheckVersion>();
            }
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            GameEntry.Event.Unsubscribe(CloseUIFormCompleteEventArgs.EventId, OnCloseUIFormCompleteEvent);
        }
        
    }
}