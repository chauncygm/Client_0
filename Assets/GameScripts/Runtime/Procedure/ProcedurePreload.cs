using GameFramework;
using GameFramework.Resource;
using UnityGameFramework.Runtime;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain
{
    /// <summary>
    /// 预加载流程
    /// </summary>
    public class ProcedurePreload : ProcedureBase
    {
        private int _loadedCount;
        private int _totalCount;

        /// <summary>
        /// 预加载回调。
        /// </summary>
        private LoadAssetCallbacks _preLoadAssetCallbacks;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            _loadedCount = 0;
            _totalCount = 0;
            _preLoadAssetCallbacks = new LoadAssetCallbacks(OnPreLoadAssetSuccess, OnPreLoadAssetFailure);

            UILoadMgr.Show(UIDefine.UILoadUpdate, Utility.Text.Format(LoadText.Instance.LabelPreLoadProgress, 0));
            PreloadResources();
        }


        private void PreloadResources()
        {
            if (GameModule.Resource.PlayMode == EPlayMode.EditorSimulateMode)
            {
                Log.Info("编辑器模拟模式，跳过预加载");
                ChangeProcedure<ProcedureLoadAssembly>();
                return;
            }

            var preLoadTags = SettingsUtils.GetPreLoadTags();
            if (preLoadTags == null || preLoadTags.Length == 0)
            {
                Log.Info("没有配置预加载标签，跳过预加载");
                ChangeProcedure<ProcedureLoadAssembly>();
                return;
            }

            var assetInfos = GameModule.Resource.GetAssetInfos(preLoadTags);
            if (assetInfos == null || assetInfos.Length == 0)
            {
                Log.Info("没有需要预加载的资源");
                ChangeProcedure<ProcedureLoadAssembly>();
                return;
            }

            _totalCount = assetInfos.Length;
            Log.Info($"开始预加载 {_totalCount} 个资源");

            foreach (var assetInfo in assetInfos)
            {
                GameModule.Resource.LoadAssetAsync(assetInfo.Address, typeof(UnityEngine.Object), _preLoadAssetCallbacks);
            }
        }


        private void OnPreLoadAssetFailure(string assetName, LoadResourceStatus status, string errormessage, object userdata)
        {
            Log.Warning("preload asset '{0}' catch error message '{1}'.", assetName, errormessage);
            OnAssetLoadComplete();
        }

        private void OnPreLoadAssetSuccess(string assetName, object asset, float duration, object userdata)
        {
            Log.Debug("Success preload asset from '{0}' duration '{1}'.", assetName, duration);
            OnAssetLoadComplete();
        }

        private void OnAssetLoadComplete()
        {
            _loadedCount++;
            
            var progress = _totalCount > 0 ? (float)_loadedCount / _totalCount * 100 : 100;
            UILoadMgr.Show(UIDefine.UILoadUpdate, Utility.Text.Format(LoadText.Instance.LabelPreLoadProgress, progress));
            
            if (_loadedCount >= _totalCount)
            {
                Log.Info($"预加载完成，共 {_totalCount} 个资源");
                UILoadMgr.Show(UIDefine.UILoadUpdate, LoadText.Instance.LabelPreLoadComplete);
                ChangeProcedure<ProcedureLoadAssembly>();
            }
        }
    }
}