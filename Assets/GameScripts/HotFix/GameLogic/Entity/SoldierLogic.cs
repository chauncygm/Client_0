using UnityGameFramework.Runtime;

namespace GameLogic.GameScripts.HotFix.GameLogic.Entity
{
    public class SoldierLogic : EntityLogic
    {
        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            Log.Info("Init soldier...");
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
            Log.Info("Recycle soldier...");
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            Log.Info("Show soldier...");
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            Log.Info("Hide soldier...");
        }
        
        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            Log.Info("Update soldier...");
        }
    }
}