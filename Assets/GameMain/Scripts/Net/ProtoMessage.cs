using GameFramework.Network;
using Google.Protobuf;

namespace GameMain.Scripts.Net
{
    public class ProtoMessage : Packet
    {
        public static readonly int EventId = typeof(ProtoMessage).GetHashCode();

        public int MsgId { get; private set; }
        public IMessage Data { get ; set;}

        public ProtoMessage(int msgId, IMessage data)
        {
            MsgId = msgId;
            Data = data;
        }

        public override void Clear()
        {
            MsgId = 0;
            Data = null;
        }

        public override int Id => EventId;
        
        
    }
}