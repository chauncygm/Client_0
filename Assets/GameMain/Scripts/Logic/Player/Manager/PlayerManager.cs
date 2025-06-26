using System;
using GameFramework.Event;
using GameMain.Scripts.Base;
using GameMain.Scripts.Logic.Event;
using GameMain.Scripts.Message;
using GameMain.Scripts.Net;
using GameMain.Scripts.Procedure;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameMain.Scripts.Logic.Player.Manager
{
    public class PlayerManager
    {
        public static readonly Lazy<PlayerManager> Instance = new(() => new PlayerManager());

        [MessageHandler]
        public static void OnSyncLoginData(SyncLoginData msg)
        {
            var player = Data.Player.Self;
            if (msg.Uid != player.Session.Uid)
            {
                Debug.Log($"登录失败，Uid不一致, {msg.Uid} != {player.Session.Uid} ");
                return;
            }

            player.Data.Uid = msg.Uid;
            player.Data.PlayerId = msg.PlayerData.PlayerId;
            player.Data.Online = true;
            Debug.Log($"登录完成 {msg.Uid}");
            GameEntry.ChangeProcedure<ProcedureMain>();
        }

        [MessageHandler]
        public static void OnHeatBeat(ResHeartbeat msg)
        {
            var player = Data.Player.Self;
            player.Session.LastHeartBeatTime = msg.Time;
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