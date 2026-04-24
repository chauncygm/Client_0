using Cysharp.Threading.Tasks;
using UnityGameFramework.Runtime;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain
{
    /// <summary>
    /// 流程 => 初始化Package。
    /// </summary>
    public class ProcedureInitPackage : ProcedureBase
    {

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            InitPackage().Forget();
        }

        private async UniTaskVoid InitPackage()
        {
            UILoadMgr.Show(UIDefine.UILoadUpdate, LoadText.Instance.LabelInitPackage);
            
            var package = YooAssets.TryGetPackage(GameModule.Resource.PackageName);
            if (package is { InitializeStatus: EOperationStatus.Succeed })
            {
                OnInitSuccess();
                return;
            }
            
            var operation = await GameModule.Resource.InitPackage();
            operation.Completed += OnInitPackageCompleted;
            
        }

        private void OnInitPackageCompleted(AsyncOperationBase obj)
        {
            if (obj.Status == EOperationStatus.Succeed)
            {
                OnInitSuccess();
                return;
            }
            
            OnInitPackageFailed(obj.Error);
        }

        private void OnInitSuccess()
        {
            Log.Info("初始化资源包成功！");
            UILoadMgr.Show(UIDefine.UILoadUpdate, LoadText.Instance.LabelInitPackageSuccess);
            ChangeProcedure<ProcedureUpdateVersion>();
        }

        private void OnInitPackageFailed(string message)
        {
            Log.Error($"初始化资源包失败: {message}");
            UILoadMgr.Show(UIDefine.UILoadUpdate, LoadText.Instance.LabelInitPackageFailed);
            UILoadMgr.ShowMessageBox(LoadText.Instance.LabelInitPackageFailedRetry, MessageShowType.RetryOrQuitTips, Retry);
        }

        private void Retry()
        {
            InitPackage().Forget();
        }
    }
}