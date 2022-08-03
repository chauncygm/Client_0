using System;
using UnityEngine;
using UnityEngine.Profiling;
using XLua;

public class DebugInfo : MonoBehaviour
{
    private const int TickFrameInterval = 1;
    private const int TickInterval = 5;
    private float _lastTickTime;
    
    private int _preFrameCount;
    private int _frameRate;

    private string _monoMemory;
    private string _unityMemory;
    private string _xLuaMemory;
    private string _totalMemory;

    private void Start()
    {
        _lastTickTime = Time.realtimeSinceStartup;
        _preFrameCount = Time.frameCount;
    }

    private void Update()
    {
        if (Time.realtimeSinceStartup - _lastTickTime < TickFrameInterval)
            return;
        _lastTickTime = Time.realtimeSinceStartup;
        _frameRate = Time.frameCount - _preFrameCount;
        _preFrameCount = Time.frameCount;
        
        if ((int)_lastTickTime % TickInterval != 0)
            return;
        _monoMemory = ConvertSize(Profiler.GetMonoUsedSizeLong());
        _unityMemory = ConvertSize(Profiler.GetTotalAllocatedMemoryLong());
        _totalMemory = ConvertSize(Profiler.GetTotalReservedMemoryLong());
        _xLuaMemory = GetXLuaMemory();
        Debug.Log($"| 总内存: {_totalMemory} | " +
                        $"unity内存: {_unityMemory} | " +
                        $"mono内存: {_monoMemory} | " +
                        $"xLua内存: {_xLuaMemory} | " +
                        $"帧率: {_frameRate} | ");
    }
    
    private static string ConvertSize(long size)
    {
        return size < 1048576 ? $"{size / 1024:N2}KB" : $"{size / 1024576:N2}MB";
    }
    
    private static string GetXLuaMemory()
    {
        var l = Launcher.LuaEnv.L;
        double size = XLua.LuaDLL.Lua.lua_gc(l, LuaGCOptions.LUA_GCCOUNT, 0)
               + XLua.LuaDLL.Lua.lua_gc(l, LuaGCOptions.LUA_GCCOUNTB, 0) / 1024;
        return size < 1024 ? $"{size:N2}KB" : $"{size / 1024:N2}MB";
    }
}
