using System;
using System.Reflection;
using GameFramework;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityGameFramework.Runtime;

namespace GameMain
{
    public abstract class ProcedureBase : GameFramework.Procedure.ProcedureBase
    {

        private IFsm<IProcedureManager> _procedureOwner;

        protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnInit(procedureOwner);
            _procedureOwner = procedureOwner;
        }
        
        /// <summary>
        /// 如果在dll中的流程代码需要热更，那么这里需要将fsm替换为新的流程
        /// 根据类型查找流程，如果包含名字相同的流程，但是程序集不同，那么应该替换为当前要切换的流程
        /// </summary>
        /// <typeparam name="T">需切换的流程类</typeparam>
        public void ChangeProcedure<T>() where T : ProcedureBase
        {
            var targetType = typeof(T);
            var allStates = _procedureOwner.GetAllStates();
            
            FsmState<IProcedureManager> targetState = null;
            var isHotUpdateScenario = false;
            Type oldStateType = null;
            
            foreach (var state in allStates)
            {
                if (state.GetType().FullName != targetType.FullName) continue;
                targetState = state;
                oldStateType = state.GetType();
                if (state.GetType().Assembly != targetType.Assembly)
                {
                    isHotUpdateScenario = true;
                    break;
                }
            }
            
            if (targetState == null)
            {
                Log.Error($"[ChangeProcedure] Procedure not found in FSM: {targetType.FullName}");
                return;
            }
            
            if (isHotUpdateScenario)
            {
                Log.Warning("[ChangeProcedure] Hot update detected!");
                Log.Warning($"[ChangeProcedure]   Target: {targetType.FullName}");
                Log.Warning($"[ChangeProcedure]   Old Assembly: {oldStateType.Assembly.GetName().Name}");
                Log.Warning($"[ChangeProcedure]   New Assembly: {targetType.Assembly.GetName().Name}");
                
                var refreshedState = TryReplaceStateInFsm(targetState, targetType);
                if (refreshedState != null)
                {
                    targetState = refreshedState;
                    Log.Info("[ChangeProcedure] ✓ Successfully replaced state instance in FSM");
                }
                else
                {
                    Log.Error("[ChangeProcedure] ✗ Failed to replace state instance");
                    Log.Error("[ChangeProcedure] Will use old instance. To apply update:");
                    Log.Error("[ChangeProcedure]   1. Switch to another procedure first");
                    Log.Error("[ChangeProcedure]   2. Then switch back to this procedure");
                }
            }
            
            ChangeState(_procedureOwner, targetState.GetType());
        }

        private FsmState<IProcedureManager> TryReplaceStateInFsm(FsmState<IProcedureManager> oldState, Type newType)
        {
            try
            {
                var statesDictField = typeof(Fsm<IProcedureManager>).GetField("m_States", BindingFlags.NonPublic | BindingFlags.Instance);
                if (statesDictField == null)
                {
                    Log.Error("[ChangeProcedure] Cannot find m_States field in Fsm class");
                    return null;
                }
                
                var statesDict = statesDictField.GetValue(_procedureOwner);
                if (statesDict == null)
                {
                    Log.Error("[ChangeProcedure] m_States dictionary is null");
                    return null;
                }
                
                var dictType = statesDict.GetType();
                var removeMethod = dictType.GetMethod("Remove", new[] { typeof(Type) });
                var addMethod = dictType.GetMethod("Add", new[] { typeof(Type), typeof(FsmState<IProcedureManager>) });
                
                if (removeMethod == null || addMethod == null)
                {
                    Log.Error("[ChangeProcedure] Cannot find Remove or Add method on dictionary");
                    return null;
                }
                
                var oldType = oldState.GetType();
                removeMethod.Invoke(statesDict, new object[] { oldType });
                
                var newInstance = Activator.CreateInstance(newType) as FsmState<IProcedureManager>;
                if (newInstance == null)
                {
                    Log.Error("[ChangeProcedure] Failed to create new instance");
                    return null;
                }
                addMethod.Invoke(statesDict, new object[] { newType, newInstance });
                
                var onInitMethod = typeof(FsmState<IProcedureManager>)
                    .GetMethod("OnInit", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                
                if (onInitMethod != null)
                {
                    onInitMethod.Invoke(newInstance, new object[] { _procedureOwner });
                    Log.Info("[ChangeProcedure] Called OnInit on new instance via reflection");
                }
                else
                {
                    Log.Warning("[ChangeProcedure] Cannot find OnInit method, skipping initialization");
                }
                
                Log.Info("[ChangeProcedure] Replaced state in FSM dictionary");
                Log.Info($"[ChangeProcedure]   Key (unchanged): {oldType.FullName}");
                Log.Info($"[ChangeProcedure]   Old Instance Assembly: {oldType.Assembly.GetName().Name}");
                Log.Info($"[ChangeProcedure]   New Instance Assembly: {newType.Assembly.GetName().Name}");
                
                return newInstance;
            }
            catch (Exception ex)
            {
                Log.Error($"[ChangeProcedure] Exception: {ex.Message}");
                Log.Error($"[ChangeProcedure] Stack: {ex.StackTrace}");
                return null;
            }
        }

        protected void SetData<T>(string name, T data) where T : Variable
        {
            _procedureOwner.SetData(name, data);
        }
    }
}
