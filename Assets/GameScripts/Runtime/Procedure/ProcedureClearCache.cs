using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain
{
    /// <summary>
    /// 流程 => 清理缓存。
    /// </summary>
    public class ProcedureClearCache : ProcedureBase
    {

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            Log.Info("开始清理未使用的缓存文件！");
            UILoadMgr.Show(UIDefine.UILoadUpdate, LoadText.Instance.LabelClearCache);

            var operation = GameModule.Resource.ClearUnusedCacheFilesAsync();
            operation.Completed += Operation_Completed;
        }


        private void Operation_Completed(YooAsset.AsyncOperationBase obj)
        {
            UILoadMgr.Show(UIDefine.UILoadUpdate, LoadText.Instance.LabelClearCacheComplete);
            ChangeProcedure<ProcedurePreload>();
        }
    }
}