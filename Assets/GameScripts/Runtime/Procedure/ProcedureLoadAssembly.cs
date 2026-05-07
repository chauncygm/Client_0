using System;
using System.Collections.Generic;
using System.IO;
#if ENABLE_HYBRIDCLR
using HybridCLR;
#endif
using UnityEngine;
using System.Reflection;
using GameFramework;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityGameFramework.Runtime;
using YooAsset;

namespace GameMain
{
    /// <summary>
    /// 流程加载器 - 代码初始化
    /// </summary>
    public class ProcedureLoadAssembly : ProcedureBase
    {
        private const string GameAppTypeName = "GameApp";
        private const string GameAppEntryMethod = "Entrance";
        
        // 元数据程序集加载
        private int _loadMetadataAssetCount;
        private bool _loadMetadataAssemblyWait;
        private int _failureMetadataAssetCount;
        private bool _loadMetadataAssemblyComplete;
        
        // 程序集加载
        private int _loadAssetCount;
        private bool _loadAssemblyWait;
        private bool _loadAssemblyComplete;
        private int _failureAssetCount;

        // 程序集及主程序集
        private Assembly _mainLogicAssembly;
        private List<Assembly> _hotfixAssemblies;
        // 防止重复调用
        private bool _assemblyLoadedAndInvoked;

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            
            _loadAssemblyComplete = false;
            _loadMetadataAssemblyComplete = false;
            _assemblyLoadedAndInvoked = false;
            _hotfixAssemblies = new List<Assembly>();
            _failureAssetCount = 0;
            _failureMetadataAssetCount = 0;

            //AOT Assembly加载原始metadata
            if (SettingsUtils.HybridCLRCustomGlobalSettings.Enable)
            {
#if !UNITY_EDITOR
                _loadMetadataAssemblyComplete = false;
                LoadMetadataForAOTAssembly();
#else
                _loadMetadataAssemblyComplete = true;
#endif
            }
            else
            {
                _loadMetadataAssemblyComplete = true;
            } 
            
            if (SettingsUtils.HybridCLRCustomGlobalSettings.Enable && GameModule.Resource.PlayMode != EPlayMode.EditorSimulateMode)
            {
                LoadHotUpdateAssemblies();
            }
            else
            {
                _mainLogicAssembly = GetMainLogicAssembly();
                _loadAssemblyComplete = true;
            }
        }

        protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            if (!_loadAssemblyComplete || !_loadMetadataAssemblyComplete) return;
            
            TryInvokeEntryMethod();
        }

        /// <summary>
        /// 尝试调用入口方法（确保只调用一次）
        /// </summary>
        private void TryInvokeEntryMethod()
        {
            if (_assemblyLoadedAndInvoked) return;
            
            if (!ValidateAndPrepareAssembly())
            {
                Log.Error("[ProcedureLoadAssembly] Assembly validation failed, cannot proceed.");
                return;
            }
            
            _assemblyLoadedAndInvoked = true;
            ChangeProcedure<ProcedureStartGame>();
            InvokeGameAppEntrance();
        }

        /// <summary>
        /// 验证程序集并准备数据
        /// </summary>
        private bool ValidateAndPrepareAssembly()
        {
            if (_mainLogicAssembly == null)
            {
#if !UNITY_EDITOR
                Log.Fatal("[ProcedureLoadAssembly] Main logic assembly is missing!");
                return false;
#else
                _mainLogicAssembly = GetMainLogicAssembly();
                if (_mainLogicAssembly == null)
                {
                    Log.Fatal("[ProcedureLoadAssembly] Cannot find main logic assembly.");
                    return false;
                }
#endif
            }
            
            return true;
        }

        /// <summary>
        /// 调用 GameApp.Entrance 入口方法
        /// </summary>
        private void InvokeGameAppEntrance()
        {
            var appType = _mainLogicAssembly.GetType(GameAppTypeName);
            if (appType == null)
            {
                Log.Fatal($"[ProcedureLoadAssembly] Type '{GameAppTypeName}' not found in assembly.");
                return;
            }

            var entryMethod = appType.GetMethod(GameAppEntryMethod);
            if (entryMethod == null)
            {
                Log.Fatal($"[ProcedureLoadAssembly] Method '{GameAppEntryMethod}' not found in type '{GameAppTypeName}'.");
                return;
            }

            try
            {
                object[] objects = { new object[] { _hotfixAssemblies } };
                entryMethod.Invoke(null, objects);
                Log.Info("[ProcedureLoadAssembly] GameApp.Entrance invoked successfully.");
            }
            catch (Exception ex)
            {
                Log.Fatal($"[ProcedureLoadAssembly] Failed to invoke entrance method: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 获取主逻辑程序集（优化版：使用 HashSet 加速查找）
        /// </summary>
        private Assembly GetMainLogicAssembly()
        {
            var hotUpdateDllSet = new HashSet<string>(SettingsUtils.HybridCLRCustomGlobalSettings.HotUpdateAssemblies);
            Assembly mainLogicAssembly = null;
            
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var assemblyName = $"{assembly.GetName().Name}.dll";
                
                // 检查是否为主逻辑 DLL
                if (mainLogicAssembly == null && 
                    string.Equals(SettingsUtils.HybridCLRCustomGlobalSettings.LogicMainDllName, assemblyName, StringComparison.Ordinal))
                {
                    mainLogicAssembly = assembly;
                }

                // 检查是否为热更新 DLL
                if (hotUpdateDllSet.Contains(assemblyName))
                {
                    _hotfixAssemblies.Add(assembly);
                }

                // 提前退出条件
                if (mainLogicAssembly != null && _hotfixAssemblies.Count == hotUpdateDllSet.Count)
                {
                    break;
                }
            }

            return mainLogicAssembly;
        }

        /// <summary>
        /// 加载热更新程序集
        /// </summary>
        private void LoadHotUpdateAssemblies()
        {
            foreach (var hotUpdateDllName in SettingsUtils.HybridCLRCustomGlobalSettings.HotUpdateAssemblies)
            {
                var assetLocation = ResolveAssetLocation(hotUpdateDllName);
                Log.Debug($"[ProcedureLoadAssembly] Loading assembly: {assetLocation}");
                _loadAssetCount++;
                GameModule.Resource.LoadAsset<TextAsset>(assetLocation, LoadAssetSuccess);
            }
            _loadAssemblyWait = true;

        }

        /// <summary>
        /// 解析资源路径
        /// </summary>
        private string ResolveAssetLocation(string dllName)
        {
            if (SettingsUtils.HybridCLRCustomGlobalSettings.EnableAddressable)
            {
                return dllName;
            }
            
            return Utility.Path.GetRegularPath(
                Path.Combine(
                    "Assets",
                    SettingsUtils.HybridCLRCustomGlobalSettings.AssemblyTextAssetPath,
                    $"{dllName}{SettingsUtils.HybridCLRCustomGlobalSettings.AssemblyTextAssetExtension}"));
        }

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// 加载代码资源成功回调。
        /// </summary>
        /// <param name="textAsset">资源操作句柄。</param>
        private void LoadAssetSuccess(TextAsset textAsset)
        {
            if (textAsset == null)
            {
                _failureAssetCount++;
                Log.Error($"[ProcedureLoadAssembly] Failed to load assembly TextAsset. Remaining: {_loadAssetCount - 1}");
                CheckAssemblyLoadComplete();
                return;
            }

            var assetName = textAsset.name;
            Log.Debug($"[ProcedureLoadAssembly] Assembly loaded: {assetName}");

            try
            {
                var assembly = Assembly.Load(textAsset.bytes);
                
                // 检查是否为主逻辑 DLL
                if (string.Equals(SettingsUtils.HybridCLRCustomGlobalSettings.LogicMainDllName, assetName, StringComparison.Ordinal))
                {
                    _mainLogicAssembly = assembly;
                    Log.Info($"[ProcedureLoadAssembly] Main logic assembly loaded: {assetName}");
                }
                _hotfixAssemblies.Add(assembly);
                Log.Debug($"[ProcedureLoadAssembly] Hotfix assembly loaded: {assembly.GetName().Name}");
            }
            catch (Exception e)
            {
                _failureAssetCount++;
                Log.Error($"[ProcedureLoadAssembly] Failed to load assembly '{assetName}': {e.Message}");
                // 不抛出异常，允许其他 DLL 继续加载
            }
            finally
            {
                _loadAssetCount--;
                CheckAssemblyLoadComplete();
            }
        }

        /// <summary>
        /// 检查程序集加载是否完成
        /// </summary>
        private void CheckAssemblyLoadComplete()
        {
            if (_loadAssemblyWait && _loadAssetCount <= 0)
            {
                _loadAssemblyComplete = true;
                Log.Info($"[ProcedureLoadAssembly] All assemblies loaded. Success: {_loadAssetCount + _failureAssetCount - _failureAssetCount}, Failed: {_failureAssetCount}");
            }
        }

        /// <summary>
        /// 为AOT Assembly加载原始metadata。
        /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行。
        /// </summary>
        public void LoadMetadataForAOTAssembly()
        {
            if (SettingsUtils.HybridCLRCustomGlobalSettings.AOTMetaAssemblies.Count == 0)
            {
                _loadMetadataAssemblyComplete = true;
                Log.Debug("[ProcedureLoadAssembly] No AOT metadata assemblies to load.");
                return;
            }

            foreach (var aotDllName in SettingsUtils.HybridCLRCustomGlobalSettings.AOTMetaAssemblies)
            {
                var assetLocation = ResolveAssetLocation(aotDllName);
                Log.Debug($"[ProcedureLoadAssembly] Loading AOT metadata: {assetLocation}");
                _loadMetadataAssetCount++;
                GameModule.Resource.LoadAsset<TextAsset>(assetLocation, LoadMetadataAssetSuccess);
            }

            _loadMetadataAssemblyWait = true;
        }

        /// <summary>
        /// 加载元数据资源成功回调。
        /// </summary>
        /// <param name="textAsset">资源操作句柄。</param>
        private void LoadMetadataAssetSuccess(TextAsset textAsset)
        {
            if (textAsset == null)
            {
                _failureMetadataAssetCount++;
                Log.Error($"[ProcedureLoadAssembly] Failed to load AOT metadata TextAsset. Remaining: {_loadMetadataAssetCount - 1}");
                CheckMetadataLoadComplete();
                return;
            }

            var assetName = textAsset.name;
            Log.Debug($"[ProcedureLoadAssembly] AOT metadata loaded: {assetName}");

            try
            {
                var dllBytes = textAsset.bytes;
#if ENABLE_HYBRIDCLR
                const HomologousImageMode mode = HomologousImageMode.SuperSet;
                var err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
                
                if (err != LoadImageErrorCode.OK)
                {
                    Log.Error($"[ProcedureLoadAssembly] LoadMetadataForAOTAssembly failed: {assetName}, mode: {mode}, error: {err}");
                    _failureMetadataAssetCount++;
                }
                else
                {
                    Log.Info($"[ProcedureLoadAssembly] AOT metadata loaded successfully: {assetName}");
                }
#endif
            }
            catch (Exception e)
            {
                _failureMetadataAssetCount++;
                Log.Error($"[ProcedureLoadAssembly] Exception loading AOT metadata '{assetName}': {e.Message}");
            }
            finally
            {
                _loadMetadataAssetCount--;
                CheckMetadataLoadComplete();
            }
        }

        /// <summary>
        /// 检查元数据加载是否完成
        /// </summary>
        private void CheckMetadataLoadComplete()
        {
            if (_loadMetadataAssemblyWait && _loadMetadataAssetCount <= 0)
            {
                _loadMetadataAssemblyComplete = true;
                Log.Info($"[ProcedureLoadAssembly] All AOT metadata loaded. Failed: {_failureMetadataAssetCount}");
            }
        }
    }
}