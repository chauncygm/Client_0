using System;
using System.Buffers.Binary;
using System.IO;
using GameFramework.Network;
using GameMain.Scripts.Logic.Player.Manager;
using Google.Protobuf;
using ICSharpCode.SharpZipLib.Checksums;
using UnityEngine;

namespace GameMain.Scripts.Net
{
    public class ClientNetWorkChannelHelper : INetworkChannelHelper
    {
        private INetworkChannel _mNetworkChannel;
        private readonly MessageRegistry _mMessageRegistry = new();
        private readonly MessageDispatcher _messageDispatcher = new();

        /// <summary>
        /// 网络包头长度固定为2
        /// </summary>
        public int PacketHeaderLength => 2;

        public void RegisterProto(string protocolNamespace)
        {
            _mMessageRegistry.RegisterProto(protocolNamespace);
        }
        
        public void RegisterHandler<T>(Action<T> handler) where T : IMessage
        {
            MessageDispatcher.RegisterHandler(handler);
        }

        public void Initialize(INetworkChannel networkChannel)
        {
            _mNetworkChannel = networkChannel;
            networkChannel.HeartBeatInterval = 3L;
            networkChannel.RegisterHandler(_messageDispatcher);
            networkChannel.SetDefaultHandler((_, packet) =>
            {
                Debug.Log($"未知的数据包: {packet.GetType().Name}");
            });
        }

        public void Shutdown()
        {
            Debug.Log($"{_mNetworkChannel.Name} 连接已关闭");
        }

        public void PrepareForConnecting()
        {
            Debug.Log($"{_mNetworkChannel.Name} 开始连接服务器...");
        }

        public bool SendHeartBeat()
        {
            return PlayerManager.SendHeartBeat();
        }

        public bool Serialize<T>(T packet, Stream destination) where T : Packet
        {
            if (packet is not ProtoMessage protoMessage) return false;
            
            var protocolIdBytes = new byte[4];
            BinaryPrimitives.WriteInt32BigEndian(protocolIdBytes, protoMessage.MsgId);
            
            var protoData = protoMessage.Data.ToByteArray();

            var crc32Bytes = new byte[8];
            var combineProtoBytes = Combine(protocolIdBytes, protoData);
            var crc32Value = CalculateCrc32(combineProtoBytes);
            BinaryPrimitives.WriteInt64BigEndian(crc32Bytes, crc32Value);


            // 写入包长度2 bit 【4 (ProtocolId) + x (ProtoData) + 8 (CRC)】
            var lengthBytes = new byte[2];
            var packetLength = (short) (4 + protoData.Length + 8);
            BinaryPrimitives.WriteInt16BigEndian(lengthBytes, packetLength);

            // 写入流
            destination.Write(lengthBytes, 0, 2);
            destination.Write(protocolIdBytes, 0, 4);
            destination.Write(protoData, 0, protoData.Length);
            destination.Write(crc32Bytes, 0, 8);
            return true;

        }

        public IPacketHeader DeserializePacketHeader(Stream source, out object customErrorData)
        {
            customErrorData = null;
            var lengthBytes = new byte[2];
            if (source.Read(lengthBytes, 0, 2) != 2)
            {
                customErrorData = "can't read packet header";
                return null;
            }
            
            int packetLength = BinaryPrimitives.ReadInt16BigEndian(lengthBytes);
            return new SimplePacketHeader(packetLength);
        }

        public Packet DeserializePacket(IPacketHeader packetHeader, Stream source, out object customErrorData)
        {
            customErrorData = null;
            if (packetHeader is not SimplePacketHeader header)
            {
                customErrorData = "invalid packet header";
                return null;
            }
            var remainingLength = header.PacketLength;


            // 读取 Proto 数据 (协议号 + x bit数据)
            var protoData = new byte[remainingLength - 8]; // 减去 CRC 的 8 bit
            if (source.Read(protoData, 0, protoData.Length) != protoData.Length)
            {
                customErrorData = "无法读取 Proto 数据";
                return null;
            }

            // 读取 CRC 校验码 (8 bit)
            var receivedCrc = new byte[8];
            if (source.Read(receivedCrc, 0, 8) != 8)
            {
                customErrorData = "无法读取 CRC 校验码";
                return null;
            }
            var crc32Value = BinaryPrimitives.ReadInt64BigEndian(receivedCrc);

            // 验证 CRC
            if (CalculateCrc32(protoData) != crc32Value)
            {
                customErrorData = "CRC 校验失败";
                return null;
            }
            
            
            // 从 protoData 前4位获取协议号
            var protocolIdBytes = new byte[4];
            Buffer.BlockCopy(protoData, 0, protocolIdBytes, 0, 4);
            var protocolId = BinaryPrimitives.ReadInt32BigEndian(protocolIdBytes);

            // 调用 Proto 类的 Parse 方法解析数据（跳过前4位协议号）
            var messageData = new byte[protoData.Length - 4];
            Buffer.BlockCopy(protoData, 4, messageData, 0, messageData.Length);

            var parser = _mMessageRegistry.GetParser(protocolId, out var parseFromMethod, out var outData);
            if (parser == null)
            {
                customErrorData = outData;
                return null;
            }

            // 调用 ParseFrom 方法
            var parsedPacket = parseFromMethod.Invoke(parser, new object[] { messageData });
            return new ProtoMessage(protocolId, parsedPacket as IMessage);
        }

        private static long CalculateCrc32(byte[] data)
        {
            var crc32 = new Crc32();
            crc32.Update(data);
            return crc32.Value;
        }
        
        private static byte[] Combine(byte[] first, byte[] second)
        {
            var combined = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, combined, 0, first.Length);
            Buffer.BlockCopy(second, 0, combined, first.Length, second.Length);
            return combined;
        }

    }
}