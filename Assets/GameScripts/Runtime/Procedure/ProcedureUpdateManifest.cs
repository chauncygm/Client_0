using UnityGameFramework.Runtime;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain
{
    /// <summary>
    /// 流程 => 用户尝试更新清单
    /// </summary>
    public class ProcedureUpdateManifest: ProcedureBase
    {

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            Log.Info("更新资源清单");
            UpdateManifest();
        }

        private void UpdateManifest()
        {
            UILoadMgr.Show(UIDefine.UILoadUpdate, LoadText.Instance.LabelUpdateManifest);
            var operation = GameModule.Resource.UpdatePackageManifestAsync(GameModule.Resource.PackageVersion);
            operation.Completed += OnOperationCompleted;
        }

        private void OnOperationCompleted(AsyncOperationBase operation)
        {
            if(operation.Status == EOperationStatus.Succeed)
            {
                if (GameModule.Resource.PlayMode == EPlayMode.WebPlayMode ||
                    GameModule.Resource.UpdatableWhilePlaying)
                {
                    // 边玩边下载还可以拓展首包支持。
                    ChangeProcedure<ProcedurePreload>();
                    return;
                }
                ChangeProcedure<ProcedureDownloader>();
            }
            else
            {
                Log.Error("更新资源清单失败：" + operation.Error);
                UILoadMgr.ShowMessageBox(LoadText.Instance.LabelUpdateManifestFailed, MessageShowType.RetryOrQuitTips, UpdateManifest);
            }
        }
    }
}