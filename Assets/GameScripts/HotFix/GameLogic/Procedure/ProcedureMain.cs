using GameFramework.Resource;
using GameLogic.GameScripts.HotFix.GameLogic.Entity;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureBase = GameMain.ProcedureBase;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameLogic
{
    public class ProcedureMain : ProcedureBase
    {

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            
            UISystem.Instance.ShowUI<MainUI>();
            Debug.Log("Main start");
            
            GameEvent.AddEventListener(IActorLogicEvent_Event.OnMainPlayerDisconnect, OnMainPlayerDisconnect);

            GameModule.Timer.AddOnceTimer(3000, () =>
            {
                GameModule.Scene.LoadScene("Assets/Res/Scenes/Temp/Tmp_1", new LoadSceneCallbacks(LoadSceneSuccessCallback));
            });
        }

        private void LoadSceneSuccessCallback(string sceneAssetName, UnityEngine.SceneManagement.Scene scene, float duration,
            object userData)
        {
            GameModule.Entity.ShowEntityAsync<SoldierLogic>(1, "Assets/Res/Prefab/Entity/Swat", "Soldier");
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            UISystem.Instance.CloseUI<LoginUI>();
            GameEvent.RemoveEventListener(IActorLogicEvent_Event.OnMainPlayerDisconnect, OnMainPlayerDisconnect);
        }

        private void OnMainPlayerDisconnect()
        {
            ChangeProcedure<ProcedureLogin>();
        }

    }
}