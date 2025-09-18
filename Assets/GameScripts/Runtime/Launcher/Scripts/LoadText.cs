using UnityEngine;

namespace GameMain
{
    public class TextMode
    {
        public string LabelLoadProgress = "正在下载资源文件，请耐心等待\n当前下载速度：{0}/s 资源文件大小：{1}";
        public string LabelLoadFirstUnpack = "首次进入游戏，正在初始化游戏资源...（此过程不消耗网络流量）";
        public string LabelLoadUnpacking = "正在更新本地资源版本，请耐心等待...（此过程不消耗网络流量）";
        public string LabelLoadChecking = "检测版本文件{0}...";
        public string LabelLoadChecked = "最新版本检测完成";
        public string LabelLoadPackage = "当前使用的版本过低，请下载安装最新版本";
        public string LabelLoadPlatform = "当前使用的版本过低，请前往应用商店安装最新版本";
        public string LabelLoadNotice = "检测到可选资源更新，推荐完成更新提升游戏体验";
        public string LabelLoadForce = "检测到版本更新，取消更新将导致无法进入游戏";

        public string LabelLoadForceWifi =
            "检测到有新的游戏内容需要更新，更新包大小<color=#BA3026>{0}</color>, 取消更新将导致无法进入游戏，您当前已为<color=#BA3026>wifi网络</color>，请开始更新";

        public string LabelLoadForceNoWifi =
            "检测到有新的游戏内容需要更新，更新包大小<color=#BA3026>{0}</color>, 取消更新将导致无法进入游戏，请开始更新";

        public string LabelLoadError = "更新参数错误{0}，请点击确定重新启动游戏";
        public string LabelLoadFirstEntrerGameError = "首次进入游戏资源异常";
        public string LabelLoadUnpackComplete = "正在加载最新资源文件...（此过程不消耗网络流量）";
        public string LabelLoadUnPackError = "资源解压失败，请点击确定重新启动游戏";
        public string LabelLoadLoadProgress = "正在载入...{0}%";
        public string LabelLoadDownloadProgress = "正在下载...{0}%";
        public string LabelLoadInit = "初始化...";
        public string LabelNetUnReachable = "当前网络不可用，请检查本地网络设置后点击确认进行重试";
        public string LabelNetReachableViaCarrierDataNetwork = "当前是移动网络，是否继续下载";
        public string LabelNetError = "网络异常，请重试";
        public string LabelNetChanged = "网络切换,正在尝试重连,{0}次";
        public string LabelDataEmpty = "数据异常";
        public string LabelMemoryLow = "初始化资源加载失败，请检查本地内存是否充足";
        public string LabelMemoryLowLoad = "内存是否充足,无法更新";
        public string LabelMemoryUnZipLow = "内存不足，无法解压";
        public string LabelAppID = "游戏版本号:{0}";
        public string LabelResID = "资源版本号:{0}";
        public string LabelClearComfirm = "是否清理本地资源?(清理完成后会关闭游戏且重新下载最新资源)";
        public string LabelRestartApp = "本次更新需要重启应用，请点击确定重新启动游戏";
        public string LabelDownLoadFailed = "网络太慢，是否继续下载";
        public string LabelClearConfig = "清除环境配置，需要重启应用";
        public string LabelRegionInfoIllegal = "区服信息为空";
        public string LabelRemoteUrlisNull = "热更地址为空";
        public string LabelFirstPackageNotFound = "首包资源加载失败";
        public string LabelRequestReginInfo = "正在请求区服信息{0}次";
        public string LabelRequestTimeOut = "请求区服信息超时,是否重试？";
        public string LabelRegionArgumentError = "参数错误";
        public string LabelRegionIndexOutOfRange = "索引越界";
        public string LabelRegionNonConfigApplication = "未配置此应用";
        public string LabelRegionSystemError = "系统异常";

        public string LabelPreventionOfAddiction =
            "著作人权：XX市TEngine有限公司 软著登记号：2022SR0000000\n抵制不良游戏，拒绝盗版游戏。注意自我保护，谨防受骗上当。适度游戏益脑，" +
            "沉迷游戏伤身。合理安排时间，享受健康生活。";

        public string LabelBtnUpdate = "确定";
        public string LabelBtnIgnore = "取消";
        public string LabelBtnPackage = "更新";

        public string LabelDlcConfigVerificateStage = "配置校验中...";
        public string LabelDlcConfigLoadingStage = "下载配置中...";
        public string LabelDlcAssetsLoading = "下载资源中...";
        public string LabelDlcLoadingFinish = "下载结束";

        public string LabelDlcLoadForceWifi =
            "检测到有新的游戏内容需要更新, 取消更新将导致无法进入游戏，您当前已为<color=#BA3026>wifi网络</color>，请开始更新";

        public string LabelDlcLoadForceNoWifi =
            "检测到有新的游戏内容需要更新, 取消更新将导致无法进入游戏，请开始更新";

        public string LabelHadUpdate = "检测到有版本更新...";
        public string LabelRequestVersionIng = "正在向服务器请求版本信息中...";
        public string LabelRequestVersionInfo = "正在向服务器请求版本信息{0}次";
    }

    public class LoadText : TextMode
    {
        private static LoadText _instance;

        public static LoadText Instance
        {
            get { return _instance ??= new LoadText(); }
        }

        public void InitConfigData(TextAsset asset)
        {
            if (asset == null)
                return;

            var loadConfig = JsonUtility.FromJson<TextMode>(asset.text);
            if (loadConfig == null) return;
            
            // 使用反射复制所有字段值
            var fields = typeof(TextMode).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var field in fields)
            {
                field.SetValue(this, field.GetValue(loadConfig));
            }
        }
    }
}