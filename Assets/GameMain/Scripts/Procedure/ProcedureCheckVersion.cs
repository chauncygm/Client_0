using System.Collections;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Runtime;
using YooAsset;

namespace GameMain.Scripts.Procedure
{
    public class ProcedureCheckVersion : BaseProcedure
    {

        private const string HostServerUrl = "http://127.0.0.1:11001/CDN";
        
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Debug.Log("开始检查版本...");
            var go = Resources.Load<GameObject>("PatchWindow");
            Object.Instantiate(go);

            Debug.Log(Boot.Instance.playMode);
            Boot.Instance.StartCoroutine(InitPackage());
        }

        private IEnumerator InitPackage()
        {
            var packageName = "DefaultPackage";
            var playMode = Boot.Instance.playMode;
            var package = YooAssets.TryGetPackage(packageName);
            if (package == null)
            {
                package = YooAssets.CreatePackage(packageName);
            }

            InitializationOperation initializationOperation = null;
            if (playMode == EPlayMode.EditorSimulateMode)
            {
                var packageInvokeBuildResult = EditorSimulateModeHelper.SimulateBuild(packageName);
                var packageRoot = packageInvokeBuildResult.PackageRootDirectory;
                var editorSimulateModeParameters = new EditorSimulateModeParameters();
                editorSimulateModeParameters.EditorFileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
                initializationOperation = package.InitializeAsync(editorSimulateModeParameters);
            }

            if (playMode == EPlayMode.OfflinePlayMode)
            {
                var offlinePlayModeParameters = new OfflinePlayModeParameters();
                offlinePlayModeParameters.BuildinFileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
                initializationOperation = package.InitializeAsync(offlinePlayModeParameters);
            }

            if (playMode == EPlayMode.HostPlayMode)
            {
                var defaultHostServer = GetHostServerUrl();
                var fallbackHostServer = GetHostServerUrl();
                var remoteService = new RemoteService(defaultHostServer, fallbackHostServer);
                var hostPlayModeParameters = new HostPlayModeParameters();
                hostPlayModeParameters.BuildinFileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
                hostPlayModeParameters.CacheFileSystemParameters = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteService);
                initializationOperation = package.InitializeAsync(hostPlayModeParameters);
            }

            if (playMode == EPlayMode.WebPlayMode)
            {
                var webPlayModeParameters = new WebPlayModeParameters();
                webPlayModeParameters.WebServerFileSystemParameters = FileSystemParameters.CreateDefaultWebServerFileSystemParameters();
                initializationOperation = package.InitializeAsync(webPlayModeParameters);
            }
            
            yield return initializationOperation;

            if (initializationOperation.Status != EOperationStatus.Succeed)
            {
                Log.Info("初始化资源失败");
            }
            else
            {
                Log.Info("初始化资源成功");
                ChangeProcedure<ProcedureLogin>();
            }
        }


        private string GetHostServerUrl()
        {
            const string appVersion = "v1.0.0";
#if UNITY_EDITOR
            return EditorUserBuildSettings.activeBuildTarget switch
            {
                BuildTarget.Android => $"{HostServerUrl}/Android/{appVersion}",
                BuildTarget.iOS => $"{HostServerUrl}/IOS/{appVersion}",
                BuildTarget.WebGL => $"{HostServerUrl}/WebGL/{appVersion}",
                _ => $"{HostServerUrl}/PC/{appVersion}"
            };
#else
            return Application.platform switch
            {
                RuntimePlatform.WindowsEditor => $"{HostServerUrl}/PC/{appVersion}",
                RuntimePlatform.OSXEditor => $"{HostServerUrl}/IOS/{appVersion}",
                _ => $"{HostServerUrl}/PC/{appVersion}"
            };
#endif
        }
        
        private class RemoteService : IRemoteServices
        {

            private readonly string _defaultHostServer;
            private readonly string _fallbackHostServer;

            public RemoteService(string defaultHostServer, string fallbackHostServer)
            {
                _defaultHostServer = defaultHostServer;
                _fallbackHostServer = fallbackHostServer;
            }

            public string GetRemoteMainURL(string fileName)
            {
                return $"{_defaultHostServer}/{fileName}";
            }

            public string GetRemoteFallbackURL(string fileName)
            {
                return $"{_fallbackHostServer}/{fileName}";
            }
        }
    }
}