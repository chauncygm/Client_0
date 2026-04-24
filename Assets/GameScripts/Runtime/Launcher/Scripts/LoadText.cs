using System;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain
{
    public class TextMode
    {
        public string LabelAppID = "游戏版本号:{0}";
        public string LabelResID = "资源版本号:{0}";
        public string LabelLoadFirstUnpack = "首次进入游戏，正在初始化游戏资源...（此过程不消耗网络流量）";
        public string LabelLoadUnpacking = "正在更新本地资源版本，请耐心等待...（此过程不消耗网络流量）";

        public string LabelInitPackage = "初始化资源包...";
        public string LabelInitPackageSuccess = "初始化资源包成功！";
        public string LabelInitPackageFailed = "资源初始化失败！";
        public string LabelInitPackageFailedRetry = "资源初始化失败，点击确认重试！";
        public string LabelUpdateVersionFile = "更新静态版本文件...";
        public string LabelRequestVersion = "正在向服务器请求版本信息中...";
        public string LabelNetUnReachable = "当前网络不可用，请检查本地网络设置后点击确认进行重试";
        public string LabelUpdateStaticVersionFileFailed = "用户尝试更新静态版本失败，点击确认重试！";
        
        public string LabelUpdateManifest = "更新资源清单文件...";
        public string LabelUpdateManifestFailed = "用户尝试更新清单失败，点击确认重试！";
        public string LabelCreatePatchDownloader = "创建补丁下载器...";
        public string LabelFoundPatch = "拉取到补丁文件{0}个，共{1}";
        public string LabelDownloadPatch = "开始下载补丁文件...";
        public string LabelDownloadDiskError = "磁盘空间不足！\n需要: {0}MB\n可用: {1}MB\n请清理空间后重试！";
        public string LabelDownloadComplete = "下载补丁文件完成";
        public string LabelDownloadFailed = "下载补丁文件失败, 请检查网络设置后重试！";
        public string LabelClearCache = "清理未使用的缓存文件...";
        public string LabelClearCacheComplete = "清理未使用的缓存文件完成";
        public string LabelPreLoadProgress = "正在载入...{0}%";
        public string LabelPreLoadComplete = "加载完成";

        public string LabelBtnUpdate = "确定";
        public string LabelBtnIgnore = "取消";
        public string LabelBtnPackage = "更新";

        public string LabelLoadInit = "初始化...";
        public string LabelHadUpdate = "检测到有版本更新...";
        public string LabelRequestVersionInfo = "正在向服务器请求版本信息{0}次";
        public string LabelLoadChecking = "检测版本文件{0}...";
        public string LabelLoadPackage = "当前使用的版本过低，请下载安装最新版本";
        public string LabelLoadPlatform = "当前使用的版本过低，请前往应用商店安装最新版本";
        public string LabelLoadForceWifi = "检测到有新的游戏内容需要更新，更新包大小<color=#BA3026>{0}</color>, 取消更新将导致无法进入游戏，您当前已为<color=#BA3026>wifi网络</color>，请开始更新";
        public string LabelLoadForceNoWifi = "检测到有新的游戏内容需要更新，更新包大小<color=#BA3026>{0}</color>, 取消更新将导致无法进入游戏，请开始更新";
        public string LabelDlcLoadForceWifi = "检测到有新的游戏内容需要更新, 取消更新将导致无法进入游戏，您当前已为<color=#BA3026>wifi网络</color>，请开始更新";
        public string LabelDlcLoadForceNoWifi = "检测到有新的游戏内容需要更新, 取消更新将导致无法进入游戏，请开始更新";
        public string LabelLoadChecked = "最新版本检测完成";
        public string LabelRestartApp = "本次更新需要重启应用，请点击确定重新启动游戏";
        public string LabelDownLoadFailed = "网络太慢，是否继续下载";
        public string LabelClearConfig = "清除环境配置，需要重启应用";
        public string LabelFirstPackageNotFound = "首包资源加载失败";
        public string LabelLoadProgress = "正在下载资源文件，请耐心等待\n当前下载速度：{0}/s 资源文件大小：{1}";
        public string LabelLoadNotice = "检测到可选资源更新，推荐完成更新提升游戏体验";
        public string LabelLoadForce = "检测到版本更新，取消更新将导致无法进入游戏";
        public string LabelLoadFirstEnterGameError = "首次进入游戏资源异常";
        public string LabelLoadUnpackComplete = "正在加载最新资源文件...（此过程不消耗网络流量）";
        public string LabelLoadUnPackError = "资源解压失败，请点击确定重新启动游戏";
        public string LabelLoadDownloadProgress = "正在下载...{0}%";
        public string LabelNetReachableViaCarrierDataNetwork = "当前是移动网络，是否继续下载";
        public string LabelNetError = "网络异常，请重试";
        public string LabelRegionSystemError = "系统异常";
        public string LabelDataEmpty = "数据异常";
        public string LabelNetChanged = "网络切换,正在尝试重连,{0}次";
        public string LabelMemoryLow = "初始化资源加载失败，请检查本地内存是否充足";
        public string LabelMemoryLowLoad = "内存是否充足,无法更新";
        public string LabelMemoryUnZipLow = "内存不足，无法解压";
        public string LabelClearConfirm = "是否清理本地资源?(清理完成后会关闭游戏且重新下载最新资源)";
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

            try
            {
                var loadConfig = JsonUtility.FromJson<TextMode>(asset.text);
                // 使用反射复制所有字段值
                var fields = typeof(TextMode).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                foreach (var field in fields)
                {
                    field.SetValue(this, field.GetValue(loadConfig));
                }
            }
            catch (Exception e)
            {
                Log.Error("解析文本配置失败：" + e);
            }
        }
    }
}