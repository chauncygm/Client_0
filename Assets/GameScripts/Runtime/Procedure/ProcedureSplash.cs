using System;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain
{
    /// <summary>
    /// 流程 => 闪屏。
    /// </summary>
    public class ProcedureSplash : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            //播放 Splash 动画
            UILoadMgr.Show(UIDefine.UISplash, new Action(OnSplashAnimationOver));
        }

        private void OnSplashAnimationOver()
        {
            //初始化资源包
            ChangeProcedure<ProcedureInitPackage>();
        }
    }
}
