using UnityEngine;

namespace GameMain
{
    public abstract class UIBase : MonoBehaviour
    {
        public abstract string Name();
        public abstract void OnEnter(params object[] param);

    }
}