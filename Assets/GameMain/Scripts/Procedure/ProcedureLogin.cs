using System;
using System.Net;
using GameFramework;
using GameFramework.Fsm;
using GameFramework.Network;
using GameFramework.Procedure;
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

        private NetworkComponent _networkComponent;

        private INetworkChannel _networkChannel;
        
        protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnInit(procedureOwner);
            _mNetworkChannelHelper = new ClientNetWorkChannelHelper();
            _mNetworkChannelHelper.RegisterProto("GameMain.Scripts.Message");
            _networkComponent = GameEntry.GetComponent<NetworkComponent>();
            var eventComponent = GameEntry.GetComponent<EventComponent>();
            // 订阅连接成功事件
            eventComponent.Subscribe(NetworkConnectedEventArgs.EventId, OnNetworkConnected);
            // 订阅连接关闭事件（包括主动关闭和异常断开）
            eventComponent.Subscribe(NetworkClosedEventArgs.EventId, OnNetworkClosed);
            // 订阅网络错误事件
            eventComponent.Subscribe(NetworkErrorEventArgs.EventId, OnNetworkError);
            // 订阅用户自定义的网络错误事件
            eventComponent.Subscribe(NetworkCustomErrorEventArgs.EventId, OnNetworkCustomError);
        }

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Debug.Log("开始登录流程...");
            _networkChannel = _networkComponent.CreateNetworkChannel("tcp-channel", ServiceType.Tcp, _mNetworkChannelHelper);
            _networkChannel.Connect(IPAddress.Parse(ServerIp), ServerPort);
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
    }
}