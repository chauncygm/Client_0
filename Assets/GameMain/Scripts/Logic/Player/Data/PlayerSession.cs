using GameFramework.Network;

namespace GameMain.Scripts.Logic.Player.Data
{
    public class PlayerSession
    {

        /// <summary>
        /// 网络通道
        /// </summary>
        public INetworkChannel Channel { get; set; }

        /// <summary>
        /// Uid
        /// </summary>
        public long Uid { get; set; }
        
        /// <summary>
        /// 玩家Id
        /// </summary>
        public long PlayerId { get; set; }
        
        /// <summary>
        /// 上次心跳的时间
        /// </summary>
        public long LastHeartBeatTime { get; set; }
    }
}