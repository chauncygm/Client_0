using System.Net;
using GameFramework;
using GameFramework.Event;
using GameFramework.Fsm;
using GameFramework.Network;
using GameFramework.Procedure;
using GameMain.Scripts.Logic.Enum;
using GameMain.Scripts.Logic.Event;
using GameMain.Scripts.Logic.Player.Data;
using GameMain.Scripts.Logic.Player.Manager;
using GameMain.Scripts.Message;
using GameMain.Scripts.Net;
using UnityEngine;
using UnityGameFramework.Runtime;
using NetworkClosedEventArgs = UnityGameFramework.Runtime.NetworkClosedEventArgs;
using NetworkConnectedEventArgs = UnityGameFramework.Runtime.NetworkConnectedEventArgs;
using NetworkCustomErrorEventArgs = UnityGameFramework.Runtime.NetworkCustomErrorEventArgs;
using NetworkErrorEventArgs = UnityGameFramework.Runtime.NetworkErrorEventArgs;

namespace GameMain.Scripts.Procedure
{
    public class ProcedureLogin : BaseProcedure
    {
        private const string ServerIp = "127.0.0.1";
        private const int ServerPort = 10001;
        private ClientNetWorkChannelHelper _mNetworkChannelHelper;

        private int _loginPanelSerialId;
        private int _waitingPanelSerialId;
        private INetworkChannel _networkChannel;

        protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnInit(procedureOwner);
            _mNetworkChannelHelper = new ClientNetWorkChannelHelper();
            _mNetworkChannelHelper.RegisterProto("GameMain.Scripts.Message");
            var eventComponent = Base.GameEntry.Event;
            // 订阅连接成功事件
            eventComponent.Subscribe(NetworkConnectedEventArgs.EventId, OnNetworkConnected);
            // 订阅连接关闭事件（包括主动关闭和异常断开）
            eventComponent.Subscribe(NetworkClosedEventArgs.EventId, OnNetworkClosed);
            // 订阅网络错误事件
            eventComponent.Subscribe(NetworkErrorEventArgs.EventId, OnNetworkError);
            // 订阅用户自定义的网络错误事件
            eventComponent.Subscribe(NetworkCustomErrorEventArgs.EventId, OnNetworkCustomError);
            
            eventComponent.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccessEvent);
            eventComponent.Subscribe(CloseUIFormCompleteEventArgs.EventId, OnCloseUIFormCompleteEvent);
            eventComponent.Subscribe(LoginEventResultArgs.EventId, OnLoginEventResult);
        }

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            _networkChannel = Base.GameEntry.Network.CreateNetworkChannel("tcp-channel", ServiceType.Tcp, _mNetworkChannelHelper);
            _loginPanelSerialId = Base.GameEntry.UI.OpenUIForm("Assets/GameMain/Prefab/UI/LoginPanel.prefab", "Default");
        }
        

        protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            Base.GameEntry.UI.CloseUIForm(_loginPanelSerialId);
        }

        private void OnOpenUIFormSuccessEvent(object sender, GameEventArgs e)
        {
            var args = (OpenUIFormSuccessEventArgs)e;
            if (args.UIForm.SerialId == _waitingPanelSerialId)
            {
                Debug.Log("waiting panel opened");
                return;
            }

            if (args.UIForm.SerialId != _loginPanelSerialId) return;
            SetData<VarInt32>("loginPanelSerialId", _loginPanelSerialId);
            Base.GameEntry.Event.Subscribe(LoginEventArgs.EventId, OnLoginEventArgs);
        }   

        private void OnCloseUIFormCompleteEvent(object sender, GameEventArgs e)
        {
            var args = (CloseUIFormCompleteEventArgs)e;
            if (args.SerialId == _waitingPanelSerialId)
            {
                Debug.Log("waiting panel closed");
            }
            if (args.SerialId == _loginPanelSerialId)
            {
                Debug.Log("login panel closed");
            }
        }
        
        private void OnLoginEventArgs(object sender, GameEventArgs e)
        {
            var loginEventArgs = (LoginEventArgs)e;
            var player = Player.Self;
            if (player.Session.Channel != null)
            {
                Debug.Log("已登录，请勿重复登录");
                return;
            }

            Debug.Log("开始登录...");
            player.Session.Uid = loginEventArgs.Uid;
            _networkChannel.Connect(IPAddress.Parse(ServerIp), ServerPort);
            _waitingPanelSerialId = Base.GameEntry.UI.OpenUIForm("Assets/GameMain/Prefab/UI/WaitingPanel.prefab", "Top");
        }

        private static void OnNetworkConnected(object sender, GameFrameworkEventArgs e)
        {
            var ne = (NetworkConnectedEventArgs)e;
            Debug.Log($"[网络事件] 连接成功: {ne.NetworkChannel.Name}");
            var player = Player.Self;
            player.Session.Channel = ne.NetworkChannel;
            PlayerManager.SendMsg(new ReqLogin { Uid = player.Session.Uid });
        }

        private static void OnNetworkClosed(object sender, GameFrameworkEventArgs e)
        {
            var ne = (NetworkClosedEventArgs)e;
            Debug.Log($"[网络事件] 连接已关闭: {ne.NetworkChannel.Name}");
            Player.Self.Session.Channel = null;
        }

        private static void OnNetworkError(object sender, GameFrameworkEventArgs e)
        {
            var ne = (NetworkErrorEventArgs)e;
            Debug.LogError($"[网络事件] 网络错误: {ne.NetworkChannel.Name}, 错误码: {ne.ErrorCode}, 异常: {ne.ErrorMessage}");
            Base.GameEntry.Event.Fire(LoginEventResultArgs.EventId, LoginEventResultArgs.Create(LoginResult.NETWORK_ERROR));
        }

        private static void OnNetworkCustomError(object sender, GameFrameworkEventArgs e)
        {
            var ne = (NetworkCustomErrorEventArgs)e;
            Debug.LogError($"[网络事件] 用户自定义网络错误: {ne.NetworkChannel.Name}, {ne.CustomErrorData}");
            Base.GameEntry.Event.Fire(LoginEventResultArgs.EventId, LoginEventResultArgs.Create(LoginResult.NETWORK_ERROR));
        }

        private void OnLoginEventResult(object sender, GameEventArgs e)
        {
            var args = (LoginEventResultArgs)e;
            Debug.Log($"login result: {args.Result}");
            Base.GameEntry.UI.CloseUIForm(_waitingPanelSerialId);
            if (args.Result == LoginResult.SUCCESS)
            {
                ChangeProcedure<ProcedureMain>();
                return;
            }
            
            Player.Self.Session.Channel = null;
            if (args.Result == LoginResult.NETWORK_ERROR)
            {
                Base.GameEntry.UI.OpenUIForm("Assets/GameMain/Prefab/UI/SystemTipPanel.prefab", "System", "网络异常，请稍后重试！");
            }
        }
    }
}