using System.Net;
using GameFramework;
using GameFramework.Fsm;
using GameFramework.Network;
using GameFramework.Procedure;
using GameProto;
using UnityEngine;
using UnityGameFramework.Runtime;
using NetworkClosedEventArgs = UnityGameFramework.Runtime.NetworkClosedEventArgs;
using NetworkConnectedEventArgs = UnityGameFramework.Runtime.NetworkConnectedEventArgs;
using NetworkCustomErrorEventArgs = UnityGameFramework.Runtime.NetworkCustomErrorEventArgs;
using NetworkErrorEventArgs = UnityGameFramework.Runtime.NetworkErrorEventArgs;
using ProcedureBase = GameMain.ProcedureBase;

namespace GameLogic
{
    public class ProcedureLogin : ProcedureBase
    {
        
        private ClientNetWorkChannelHelper _mNetworkChannelHelper;
        private INetworkChannel _networkChannel;

        protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnInit(procedureOwner);
            _mNetworkChannelHelper = new ClientNetWorkChannelHelper();
            _mNetworkChannelHelper.RegisterProto("GameProto", "GameProto");
        }

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            _networkChannel = GameModule.Network.CreateNetworkChannel("tcp-channel", ServiceType.Tcp, _mNetworkChannelHelper);
            
            var eventComponent = GameModule.Event;
            // 订阅连接成功事件
            eventComponent.Subscribe(NetworkConnectedEventArgs.EventId, OnNetworkConnected);
            // 订阅连接关闭事件（包括主动关闭和异常断开）
            eventComponent.Subscribe(NetworkClosedEventArgs.EventId, OnNetworkClosed);
            // 订阅网络错误事件
            eventComponent.Subscribe(NetworkErrorEventArgs.EventId, OnNetworkError);
            // 订阅用户自定义的网络错误事件
            eventComponent.Subscribe(NetworkCustomErrorEventArgs.EventId, OnNetworkCustomError);
            
            GameEvent.AddEventListener<long>(ILoginUI_Event.OnRoleLogin, OnLoginEventArgs);
            GameEvent.AddEventListener(IActorLogicEvent_Event.OnMainPlayerLoginSuccess, OnLoginEventResult);
            
            UISystem.Instance.ShowUIAsync<LoginUI>();
        }

        protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            UISystem.Instance.CloseUI<LoginUI>();
            GameEvent.RemoveEventListener<long>(ILoginUI_Event.OnRoleLogin, OnLoginEventArgs);
            GameEvent.RemoveEventListener(IActorLogicEvent_Event.OnMainPlayerLoginSuccess, OnLoginEventResult);
        }

        private void OnLoginEventArgs(long uid)
        {
            var player = Player.Self;
            if (player.Session.Channel != null)
            {
                Debug.Log("已登录，请勿重复登录");
                return;
            }

            Debug.Log("开始登录...");
            player.Session.Uid = uid;
            var serverInfo = SettingsUtils.GetServerIpAndPort();
            _networkChannel.Connect(IPAddress.Parse(serverInfo.Ip), serverInfo.Port);
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
        }

        private static void OnNetworkCustomError(object sender, GameFrameworkEventArgs e)
        {
            var ne = (NetworkCustomErrorEventArgs)e;
            Debug.LogError($"[网络事件] 用户自定义网络错误: {ne.NetworkChannel.Name}, {ne.CustomErrorData}");
        }

        private void OnLoginEventResult()
        {
            ChangeProcedure<ProcedureMain>();
        }
    }
}