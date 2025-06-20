using System;
using UnityEngine;
using XLua;

namespace GameMain.Scripts
{
    /**
     * <summary>启动脚本</summary>
     */
    public class Launcher : MonoBehaviour
    {
        private const float TickInterval = 2;
        private static float _lastTickTime;

        private LuaTable _scriptEnv;
        public static readonly LuaEnv LuaEnv = new LuaEnv();

        private Action _luaStart;
        private Action _luaUpdate;
        private Action _luaFixUpdate;
        private Action _luaLateUpdate;
        private Action _luaDestroy;

        private void Awake()
        {
            Debug.Log("Launcher awake!!");
            DontDestroyOnLoad(gameObject);
            // 启动时先加载资源 
            ResourceManager.Instance.Init(StartGame);
#if UNITY_EDITOR
            gameObject.AddComponent<DebugInfo>();
#endif
        }

        private void StartGame()
        {
            Debug.Log("Start game!!");
            // 指定加载器，当脚本有require的时候如何加载
            LuaEnv.AddLoader(FileUtils.RequireLua);
            _scriptEnv = LuaEnv.NewTable();
            // 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
            var meta = LuaEnv.NewTable();
            meta.Set("__index", LuaEnv.Global);
            _scriptEnv.SetMetaTable(meta);
            meta.Dispose();
            _scriptEnv.Set("self", this);
        
            // 加载启动类
            const string luaLauncher = "LuaLauncher";
            LuaEnv.DoString("require '" + luaLauncher + "'");
            // LuaEnv.DoString(FileUtils.RequireLua(ref luaLauncher), luaLauncher, _scriptEnv);

            _scriptEnv.Get("start", out _luaStart);
            _scriptEnv.Get("update", out _luaUpdate);
            _scriptEnv.Get("fixUpdate", out _luaFixUpdate);
            _scriptEnv.Get("lateUpdate", out _luaLateUpdate);
            _scriptEnv.Get("destroy", out _luaDestroy);
            _scriptEnv.Get<Action>("awake")?.Invoke();
        }

        private void Start()
        {
            Debug.Log("Launcher start!!");
            _luaStart?.Invoke();
        }

        private void Update()
        {
            if (Time.time - _lastTickTime <= TickInterval)
                return;
            _luaUpdate?.Invoke();
            _lastTickTime = Time.time;
        }

        private void FixedUpdate()
        {
            _luaFixUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            _luaLateUpdate?.Invoke();
        }

        private void OnDestroy()
        {
            Debug.Log("Launcher destroyed!!");
            _luaDestroy?.Invoke();
            _luaStart = null;
            _luaUpdate = null;
            _luaFixUpdate = null;
            _luaLateUpdate = null;
            _luaDestroy = null;
            LuaEnv.Dispose();
            // TODO  关闭服务器连接
        }
    }
}
