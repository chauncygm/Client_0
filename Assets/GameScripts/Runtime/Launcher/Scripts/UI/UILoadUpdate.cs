using GameFramework;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace GameMain
{
    public class UILoadUpdate : UIBase
    {
        public Button btnClear;
        public Scrollbar objProgress;
        public Text labelDesc;
        public Text labelAppId;
        public Text labelResId;

        public virtual void Start()
        {
            btnClear.onClick.AddListener(OnClear);
            btnClear.gameObject.SetActive(true);
        }

        public virtual void OnEnable()
        {
            RefreshVersion();
            GameEvent.AddEventListener<float>(StringId.StringToHash("DownProgress"),DownLoad_Progress_Action);
        }

        public override void OnEnter(object param)
        {
            base.OnEnter(param);
            labelDesc.text = param.ToString();
            RefreshVersion();
        }

        public virtual void Update()
        {
        }

        private void RefreshVersion()
        {
            labelAppId.text = string.Format(LoadText.Instance.LabelAppID, Version.GameVersion);
            labelResId.text = string.Format(LoadText.Instance.LabelResID, GameModule.Resource.PackageVersion);
        }

        protected virtual void OnContinue(GameObject obj)
        {
            // LoadMgr.Instance.StartDownLoad();
        }

        protected virtual void OnStop(GameObject obj)
        {
            // LoadMgr.Instance.StopDownLoad();
        }

        /// <summary>
        /// 清空本地缓存
        /// </summary>
        protected virtual void OnClear()
        {
            OnStop(null);
            UILoadTip.ShowMessageBox(LoadText.Instance.LabelClearComfirm, MessageShowType.TwoButton,
                LoadStyle.StyleEnum.Style_Clear,
                () =>
                {
                    GameModule.Resource.ClearUnusedCacheFilesAsync();
                    Application.Quit();
                }, () => { OnContinue(null); });
        }

        /// <summary>
        /// 下载进度完成
        /// </summary>
        /// <param name="type"></param>
        public virtual void DownLoad_Complete_Action(int type)
        {
            Log.Info("DownLoad_Complete");
        }

        /// <summary>
        /// 下载进度更新
        /// </summary>
        /// <param name="progress"></param>
        public virtual void DownLoad_Progress_Action(float progress)
        {
            objProgress.gameObject.SetActive(true);

            objProgress.size = progress;
        }

        /// <summary>
        /// 解压缩完成回调
        /// </summary>
        /// <param name="type"></param>
        /// <param name="status"></param>
        public virtual void Unpacked_Complete_Action(bool type, GameStatus status)
        {
            objProgress.gameObject.SetActive(true);
            labelDesc.text = LoadText.Instance.LabelLoadUnpackComplete;
            if (status == GameStatus.AssetLoad)
            {
            }
            else
            {
                Log.Error("error type");
            }
        }

        /// <summary>
        /// 解压缩进度更新
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="status"></param>
        public virtual void Unpacked_Progress_Action(float progress, GameStatus status)
        {
            objProgress.gameObject.SetActive(true);
            labelDesc.text = status == GameStatus.First ? LoadText.Instance.LabelLoadFirstUnpack : LoadText.Instance.LabelLoadUnpacking;

            objProgress.size = progress;
        }

        public virtual void OnDisable()
        {
            GameEvent.RemoveEventListener<float>(StringId.StringToHash("DownProgress"),DownLoad_Progress_Action);
            OnStop(null);
        }
    }
}