using UnityEngine;

namespace GameMain.Scripts.Utils
{
    public abstract class Singleton<T> where T : class, new()
    {
        private static T _instance;
        
        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (typeof(T))
                {
                    _instance ??= new T();
                }
                return _instance;
            }
        }
    }
    
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>, new()
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = FindAnyObjectByType<T>();
                if (_instance != null) return _instance;
                
                var singletonObject = new GameObject(typeof(T).Name);
                _instance = singletonObject.AddComponent<T>();
                singletonObject.name = typeof(T).Name + "(Singleton)";
                DontDestroyOnLoad(singletonObject);
                return _instance;
            }
        }

        private void Awake()
        {
            InitSingleton();
            OnInit();
        }

        private void InitSingleton()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            OnDispose();
            if (this == _instance)
            {
                _instance = null;
            }
        }

        protected virtual void OnInit()
        {
            // 子类的初始化逻辑
        }
        
        protected virtual void OnDispose()
        {
            // 子类的清理逻辑
        }
    }
}