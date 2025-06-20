using GameFramework.Network;

namespace GameMain.Scripts.Net
{
    internal class SimplePacketHeader : IPacketHeader
    {
        public int PacketLength { get; }
        
        public SimplePacketHeader(int packetLength)
        {
            PacketLength = packetLength;
        }
    }
}