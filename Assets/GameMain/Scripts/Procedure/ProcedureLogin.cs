using System;
using System.Net;
using GameFramework;
using GameFramework.Fsm;
using GameFramework.Network;
using GameFramework.Procedure;
using GameMain.Scripts.Net;
using UnityEngine;
using UnityGameFramework.Runtime;
using NetworkClosedEventArgs = UnityGameFramework.Runtime.NetworkClosedEventArgs;
using NetworkConnectedEventArgs = UnityGameFramework.Runtime.NetworkConnectedEventArgs;
using NetworkErrorEventArgs = UnityGameFramework.Runtime.NetworkErrorEventArgs;

namespace GameMain.Scripts.Procedure
{
    public class ProcedureLogin : ProcedureBase
    {
        private const string MServerIp = "127.0.0.1";
        private const int MServerPort = 10001;
        private ClientNetWorkChannelHelper _mNetworkChannelHelper;

        private EventComponent _eventComponent;
        private NetworkComponent _networkComponent;

        private INetworkChannel _networkChannel;
        
        protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnInit(procedureOwner);
            _mNetworkChannelHelper = new ClientNetWorkChannelHelper();
            _mNetworkChannelHelper.RegisterProto("GameMain.Scripts.Message");
            _networkComponent = GameEntry.GetComponent<NetworkComponent>();
            _eventComponent = GameEntry.GetComponent<EventComponent>();
            
            // 订阅连接成功事件
            _eventComponent.Subscribe(NetworkConnectedEventArgs.EventId, OnNetworkConnected);
            // 订阅连接关闭事件（包括主动关闭和异常断开）
            _eventComponent.Subscribe(NetworkClosedEventArgs.EventId, OnNetworkClosed);
            // 订阅网络错误事件
            _eventComponent.Subscribe(NetworkErrorEventArgs.EventId, OnNetworkError);

        }

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Debug.Log("开始登录流程");
            _networkChannel = _networkComponent.CreateNetworkChannel("tcp-channel", ServiceType.Tcp, _mNetworkChannelHelper);
            _networkChannel.Connect(IPAddress.Parse(MServerIp), MServerPort);
        }

        private void OnNetworkConnected(object sender, GameFrameworkEventArgs e)
        {
            var ne = (NetworkConnectedEventArgs)e;
            Debug.Log($"[网络事件] 连接成功: {ne.NetworkChannel.Name}");
        }

        private void OnNetworkClosed(object sender, GameFrameworkEventArgs e)
        {
            var ne = (NetworkClosedEventArgs)e;
            Debug.Log($"[网络事件] 连接已关闭: {ne.NetworkChannel.Name}");
        }

        private void OnNetworkError(object sender, GameFrameworkEventArgs e)
        {
            var ne = (NetworkErrorEventArgs)e;
            Debug.LogError($"[网络事件] 网络错误: {ne.NetworkChannel.Name}, 错误码: {ne.ErrorCode}, 异常: {ne.ErrorMessage}");
        }
    }
}