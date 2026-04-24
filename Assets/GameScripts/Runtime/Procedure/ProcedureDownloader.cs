using System;
using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain
{
    public class ProcedureDownloader : ProcedureBase
    {
        private float _lastUpdateDownloadedSize;
        private float _lastUpdateTime;
        private float _currentSpeed;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            UILoadMgr.Show(UIDefine.UILoadUpdate, LoadText.Instance.LabelCreatePatchDownloader);
            
            var downloader = GameModule.Resource.CreateResourceDownloader();
            Log.Info($"{downloader.TotalDownloadCount}个文件需要下载，总大小: {Utility.File.GetByteLengthString(downloader.TotalDownloadBytes)}");

            if (downloader.TotalDownloadCount == 0)
            {
                ChangeProcedure<ProcedureClearCache>();
                return;
            }

            // 检查磁盘空间
            if (!CheckDiskSpace(downloader.TotalDownloadBytes))
            {
                return;
            }

            // 显示下载确认对话框
            var totalSize = Utility.File.GetByteLengthString(downloader.TotalDownloadBytes);
            var text = Utility.Text.Format(LoadText.Instance.LabelFoundPatch, downloader.TotalDownloadCount, totalSize);
            UILoadMgr.ShowMessageBox(text, MessageShowType.RetryOrQuitTips, StartDownload);
        }

        /// <summary>
        /// 检查磁盘空间是否充足
        /// </summary>
        private static bool CheckDiskSpace(long requiredBytes)
        {
            try
            {
                // 预留额外空间用于临时文件、版本备份和文件系统开销
                var requiredSpaceWithBuffer = (long)(requiredBytes * 1.2f);
                
                var availableSpace = GetAvailableDiskSpace();
                
                if (availableSpace > 0 && availableSpace < requiredSpaceWithBuffer)
                {
                    var requiredMb = requiredSpaceWithBuffer / 1024 / 1024;
                    var availableMb = availableSpace / 1024 / 1024;
                    
                    Log.Error($"磁盘空间不足！需要: {requiredMb}MB, 可用: {availableMb}MB");

                    var text = Utility.Text.Format(LoadText.Instance.LabelDownloadDiskError, requiredMb, availableMb);
                    UILoadMgr.ShowMessageBox(text, MessageShowType.Quit);
                    
                    return false;
                }
                
                return true;
            }
            catch (Exception e)
            {
                Log.Warning($"无法检查磁盘空间: {e.Message}");
                return true; // 检查失败时继续，避免阻塞流程
            }
        }

        /// <summary>
        /// 获取可用磁盘空间（字节）
        /// </summary>
        private static long GetAvailableDiskSpace()
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            try
            {
                var driveInfo = new System.IO.DriveInfo(Application.persistentDataPath);
                return driveInfo.AvailableFreeSpace;
            }
            catch
            {
                return -1; // 无法获取时返回-1
            }
#else
            // 移动端难以准确获取，返回-1表示跳过检查
            return -1;
#endif
        }

        private void StartDownload()
        {
            UILoadMgr.Show(UIDefine.UILoadUpdate, LoadText.Instance.LabelDownloadPatch);

            // 重置速度计算变量
            _lastUpdateDownloadedSize = 0;
            _lastUpdateTime = Time.realtimeSinceStartup;
            _currentSpeed = 0;

            var downloader = GameModule.Resource.Downloader;
            downloader.DownloadErrorCallback = OnDownloadErrorCallback;
            downloader.DownloadUpdateCallback = OnDownloadProgressCallback;
            downloader.DownloadFinishCallback = OnDownloadCompleteCallback;
            downloader.BeginDownload();
        }

        private void OnDownloadCompleteCallback(DownloaderFinishData data)
        {
            if (data.Succeed)
            {
                Log.Info("下载完成");
                UILoadMgr.Show(UIDefine.UILoadUpdate, LoadText.Instance.LabelDownloadComplete);
                ChangeProcedure<ProcedureClearCache>();
            }
        }

        private void OnDownloadErrorCallback(DownloadErrorData data)
        {
            Log.Error($"下载文件失败：{data.FileName}, 错误: {data.ErrorInfo}");
            UILoadMgr.ShowMessageBox(LoadText.Instance.LabelDownloadFailed, MessageShowType.RetryOrQuitTips, StartDownload);
        }

        private void OnDownloadProgressCallback(DownloadUpdateData data)
        {
            // 计算下载速度（基于时间差）
            UpdateDownloadSpeed(data.CurrentDownloadBytes);
            
            var descriptionText = Utility.Text.Format(
                "正在更新 {0}/{1} | {2}/{3} | 进度: {4}% | 速度: {5}/s",
                data.CurrentDownloadCount,
                data.TotalDownloadCount,
                Utility.File.GetByteLengthString(data.CurrentDownloadBytes),
                Utility.File.GetByteLengthString(data.TotalDownloadBytes),
                (int)(GameModule.Resource.Downloader.Progress * 100),
                Utility.File.GetLengthString((int)_currentSpeed));
            
            GameEvent.Send(StringId.StringToHash("DownProgress"), GameModule.Resource.Downloader.Progress);
            UILoadMgr.Show(UIDefine.UILoadUpdate, descriptionText);

            // 计算剩余时间
            if (_currentSpeed > 0)
            {
                var remainingBytes = data.TotalDownloadBytes - data.CurrentDownloadBytes;
                var needTime = (int)(remainingBytes / _currentSpeed);
                var ts = new TimeSpan(0, 0, needTime);
                var timeStr = ts.ToString(@"mm\:ss");
                
                // 只在速度稳定后显示剩余时间（避免初期波动）
                if (data.CurrentDownloadBytes > data.TotalDownloadBytes * 0.05f)
                {
                    Log.Debug($"剩余时间: {timeStr} ({Utility.File.GetLengthString((int)_currentSpeed)}/s)");
                }
            }
        }

        /// <summary>
        /// 更新下载速度（基于时间差计算）
        /// </summary>
        private void UpdateDownloadSpeed(long currentBytes)
        {
            var currentTime = Time.realtimeSinceStartup;
            var timeDiff = currentTime - _lastUpdateTime;
            
            // 至少间隔 0.5 秒才更新速度，避免抖动
            if (timeDiff >= 0.5f)
            {
                var bytesDiff = currentBytes - _lastUpdateDownloadedSize;
                _currentSpeed = bytesDiff / timeDiff;
                
                _lastUpdateDownloadedSize = currentBytes;
                _lastUpdateTime = currentTime;
            }
        }
    }
}