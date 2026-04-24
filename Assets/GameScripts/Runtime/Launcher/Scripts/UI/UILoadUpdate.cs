using GameFramework;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace GameMain
{
    public class UILoadUpdate : UIBase
    {
        public Scrollbar objProgress;
        public Text labelDesc;
        public Text labelAppId;
        public Text labelResId;

        public override string Name()
        {
            return UIDefine.UILoadUpdate;
        }
        
        public void OnEnable()
        {
            GameEvent.AddEventListener<float>(StringId.StringToHash("DownProgress"), DownLoad_Progress_Action);
        }

        public override void OnEnter(params object[] param)
        {
            labelDesc.text = param[0].ToString();
            labelAppId.text = string.Format(LoadText.Instance.LabelAppID, Version.GameVersion);
            labelResId.text = string.Format(LoadText.Instance.LabelResID, GameModule.Resource.PackageVersion);
        }

        /// <summary>
        /// 下载进度更新
        /// </summary>
        /// <param name="progress"></param>
        private void DownLoad_Progress_Action(float progress)
        {
            objProgress.gameObject.SetActive(true);
            objProgress.size = progress;
        }

        /// <summary>
        /// 解压缩进度更新
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="status"></param>
        public void Unpacked_Progress_Action(float progress, GameStatus status)
        {
            objProgress.gameObject.SetActive(true);
            labelDesc.text = status == GameStatus.First ? LoadText.Instance.LabelLoadFirstUnpack : LoadText.Instance.LabelLoadUnpacking;
            objProgress.size = progress;
        }

        public void OnDisable()
        {
            GameEvent.RemoveEventListener<float>(StringId.StringToHash("DownProgress"),DownLoad_Progress_Action);
        }
    }
}