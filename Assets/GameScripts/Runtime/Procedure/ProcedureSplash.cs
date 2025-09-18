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
            //热更新UI初始化
            UILoadMgr.Initialize();
            //播放 Splash 动画
            UILoadMgr.Show(UIDefine.UISplash, new Action(OnSplashAnimationOver));
        }

        private void OnSplashAnimationOver()
        {
            //热更新阶段文本初始化
            LoadText.Instance.InitConfigData(null);
            //初始化资源包
            ChangeState<ProcedureInitPackage>(ProcedureOwner);
        }
    }
}
