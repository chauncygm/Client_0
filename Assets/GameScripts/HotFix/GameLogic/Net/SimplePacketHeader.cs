using GameFramework.Network;

namespace GameLogic.GameScripts.HotFix.GameLogic.Net
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