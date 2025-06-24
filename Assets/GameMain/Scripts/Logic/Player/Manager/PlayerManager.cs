using System;
using GameMain.Scripts.Message;
using GameMain.Scripts.Net;
using Google.Protobuf;
using UnityEngine;

namespace GameMain.Scripts.Logic.Player.Manager
{
    public class PlayerManager
    {
        public static readonly Lazy<PlayerManager> Instance = new(() => new PlayerManager());

        static PlayerManager()
        {
            InitPlayer();
        }

        private static void InitPlayer()
        {
            var player = Data.Player.Self;
            player.Data.Uid = 1001;
        }

        [MessageHandler]
        public static void OnHeatBeat(ResHeartbeat msg)
        {
            Debug.Log($"OnHeatBeat: {msg}");
            var player = Data.Player.Self;
            player.Session.LastHeartBeatTime = msg.Time;
        }

        [MessageHandler]
        public void OnSyncLoginData(SyncLoginData msg)
        {
            Debug.Log($"OnSyncLoginData：{msg}");
            var player = Data.Player.Self;
            player.Data.Online = true;
            player.Data.Uid = msg.Uid;
            player.Data.PlayerId = msg.PlayerData.PlayerId;
        }

        public static bool SendHeartBeat()
        {
            if (!Data.Player.Self.Data.Online)
            {
                return false;
            }
            SendMsg(new ReqHeartbeat());
            return true;
        }

        public static void SendMsg(IMessage message)
        {
            var player = Data.Player.Self;
            var channel = player.Session.Channel;
            var protoId = MessageRegistry.GetProtoEnum(message.GetType().Name);
            if (channel.Connected)
            {
                channel.Send(new ProtoMessage(protoId, message));
            }
            else
            {
                Debug.Log($"连接已断开，发送消息{message}失败");
            }
        }
    }
}