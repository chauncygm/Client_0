using System;
using System.Collections.Generic;
using System.Linq;
using GameProto;
using Google.Protobuf;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameLogic
{
    public class PlayerManager
    {
        public static readonly Lazy<PlayerManager> Instance = new(() => new PlayerManager());

        [MessageHandler]
        public static void OnSyncLoginData(SyncLoginData msg)
        {
            var player = Player.Self;
            if (msg.Uid != player.Session.Uid)
            {
                Debug.Log($"登录失败，Uid不一致, {msg.Uid} != {player.Session.Uid} ");
                return;
            }

            player.Data.Uid = msg.Uid;
            player.Data.PlayerId = msg.PlayerData.PlayerId;
            player.Data.Name = msg.PlayerData.Name;
            player.Data.Online = true;
            player.Data.LevelExp = new LevelExpInfo
            {
                Level = msg.PlayerData.Level,
                Exp = msg.PlayerData.Exp
            };
            var playerDataResources = msg.PlayerData.Resources;
            var dataResources = new Dictionary<int, int>();
            playerDataResources?.ToList().ForEach(x => dataResources[x.Key] = x.Value);
            player.Data.Resources = dataResources;

            
            Debug.Log($"登录完成 {msg.Uid}");
            
            GameEvent.EventMgr.GetInterface<IActorLogicEvent>().OnMainPlayerLoginSuccess();
            // GameModule.Event.Fire(LoginEventResultArgs.EventId, LoginEventResultArgs.Create(LoginResult.SUCCESS));
        }

        [MessageHandler]
        public static void OnHeatBeat(ResHeartbeat msg)
        {
            var player = Player.Self;
            player.Session.LastHeartBeatTime = msg.Time;
        }

        [MessageHandler]
        public static void OnResGm(ResGm msg)
        {
            if (msg.Status == CStatus.Success)
            {
                Debug.Log($"GM执行成功，{msg.Data}");
            }
            else
            {
                Debug.LogWarning($"GM执行失败，{msg.Message}");
            }
        }
        
        

        [MessageHandler]
        public static void OnSyncResourceChange(SyncResourceChange msg)
        {
            var player = Player.Self;
            var dataResources = player.Data.Resources;
            foreach (var resourceChange in msg.Changes)
            {
                var lastNum = dataResources.GetValueOrDefault(resourceChange.ResourceId, 0);
                if (lastNum + resourceChange.ChangeNum != resourceChange.CurrentNum)
                {
                    Debug.Log($"资源变化错误，资源id:{resourceChange.ResourceId}，变化数量:{resourceChange.ChangeNum}，当前数量:{resourceChange.CurrentNum}，上次数量:{lastNum}");
                }
                dataResources[resourceChange.ResourceId] = resourceChange.CurrentNum;
                GameEvent.EventMgr.GetInterface<IBagLogicEvent>().OnResourceChange(resourceChange.ResourceId, resourceChange.CurrentNum);
            }
        }


        [MessageHandler]
        public static void OnPlayerLevelChange(SyncLevelExpChange msg)
        {
            var player = Player.Self;
            player.Data.LevelExp = new LevelExpInfo
            {
                Level = msg.Level,
                Exp = msg.Exp
            };
            GameEvent.EventMgr.GetInterface<IActorLogicEvent>().OnMainPlayerLevelChange();
        }

        public static bool SendHeartBeat()
        {
            if (!Player.Self.Data.Online)
            {
                return false;
            }
            SendMsg(new ReqHeartbeat());
            return true;
        }

        public static void SendGM(string cmd, string param)
        {
            var reqGm = new ReqGm
            {
                Cmd = cmd,
                Params = { param.Split(" ") }
            };
            SendMsg(reqGm);
        }

        public static void SendMsg(IMessage message)
        {
            var player = Player.Self;
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