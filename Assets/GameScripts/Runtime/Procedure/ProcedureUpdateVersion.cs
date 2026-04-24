using UnityEngine;
using UnityGameFramework.Runtime;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain
{
    /// <summary>
    /// 流程 => 用户尝试更新静态版本
    /// </summary>
    public class ProcedureUpdateVersion : ProcedureBase
    {

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            UILoadMgr.Show(UIDefine.UILoadUpdate, LoadText.Instance.LabelUpdateVersionFile);
            GetStaticVersion();
        }

        /// <summary>
        /// 向用户尝试更新静态版本。
        /// </summary>
        private void GetStaticVersion()
        {
            //检查设备是否能够访问互联网
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                UILoadMgr.ShowMessageBox(LoadText.Instance.LabelNetUnReachable, MessageShowType.OkOrCancel,
                    GetStaticVersion,
                    ChangeProcedure<ProcedurePreload>);
                return;
            }
            var operation = GameModule.Resource.RequestPackageVersionAsync();
            operation.Completed += OnRequestPackageVersionCompleted;
            UILoadMgr.Show(UIDefine.UILoadUpdate, LoadText.Instance.LabelRequestVersion);
        }

        private void OnRequestPackageVersionCompleted(AsyncOperationBase obj)
        {
            if (obj is not RequestPackageVersionOperation operation) return;
            if (operation.Status == EOperationStatus.Succeed)
            {
                Log.Debug($"更新静态版本号完成，当前: {GameModule.Resource.PackageVersion} ，远端：{operation.PackageVersion}");
                GameModule.Resource.PackageVersion = operation.PackageVersion;
                ChangeProcedure<ProcedureUpdateManifest>();
            }
            else
            {
                Log.Error("更新静态版本号失败：" + operation.Error);
                UILoadMgr.ShowMessageBox(LoadText.Instance.LabelUpdateStaticVersionFileFailed, MessageShowType.RetryOrQuitTips, GetStaticVersion);
            }
        }

    }
}