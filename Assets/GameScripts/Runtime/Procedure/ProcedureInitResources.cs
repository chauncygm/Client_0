using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain
{
    public class ProcedureInitResources : ProcedureBase
    {
        private bool _initResourcesComplete;

        public override bool UseNativeDialog => true;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            _initResourcesComplete = false;
            
            UILoadMgr.Show(UIDefine.UILoadUpdate,"初始化资源中...");

            // 注意：使用单机模式并初始化资源前，需要先构建 AssetBundle 并复制到 StreamingAssets 中，否则会产生 HTTP 404 错误
            OnInitResourcesComplete();
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (_initResourcesComplete)
            {
                ChangeState<ProcedurePreload>(procedureOwner);
            }

        }

        private void OnInitResourcesComplete()
        {
            _initResourcesComplete = true;
            Log.Info("Init resources complete.");
        }
    }
}
