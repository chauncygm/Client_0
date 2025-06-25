using GameFramework.Network;

namespace GameMain.Scripts.Logic.Player.Data
{
    public class PlayerSession
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public long Uid { get; set; }
        
        /// <summary>
        /// 网络通道
        /// </summary>
        public INetworkChannel Channel { get; set; }
        
        /// <summary>
        /// 上次心跳的时间
        /// </summary>
        public long LastHeartBeatTime { get; set; }
    }
}