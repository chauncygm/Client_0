using UnityEngine;
using UnityEngine.Profiling;
using XLua;
using XLua.LuaDLL;

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


    private Rect m_fps = new Rect(10, 10, 30, 30);
    private Rect m_totalMemery = new Rect(10, 30, 30, 30);
    private GUIStyle m_style = new GUIStyle();

    private void Start()
    {
        _lastTickTime = Time.realtimeSinceStartup;
        _preFrameCount = Time.frameCount;
        
        _monoMemory = ConvertSize(Profiler.GetMonoUsedSizeLong());
        _unityMemory = ConvertSize(Profiler.GetTotalAllocatedMemoryLong());
        _totalMemory = ConvertSize(Profiler.GetTotalReservedMemoryLong());
        _xLuaMemory = GetXLuaMemory();
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
        double size = Lua.lua_gc(l, LuaGCOptions.LUA_GCCOUNT, 0)
               + Lua.lua_gc(l, LuaGCOptions.LUA_GCCOUNTB, 0) / 1024;
        return size < 1024 ? $"{size:N2}KB" : $"{size / 1024:N2}MB";
    }

    private void OnGUI()
    {
        GUI.Label(m_fps, "帧率: " + _frameRate, m_style);
        GUI.Label(m_totalMemery, "总内存: " + _totalMemory, m_style);
    }
}
