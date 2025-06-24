namespace GameMain.Scripts.Logic.Player.Data
{
    public class PlayerData
    {
        /// <summary>
        /// 账号id
        /// </summary>
        public long Uid { get; set;}
        
        /// <summary>
        /// 玩家id
        /// </summary>
        public long PlayerId { get; set;}
        
        /// <summary>
        /// 玩家名称
        /// </summary>
        public string Name { get; set;}
        
        /// <summary>
        /// 在线状态
        /// </summary>
        public bool Online { get; set;}
        
        /// <summary>
        /// 经验等级
        /// </summary>
        public LevelExpInfo LevelExp { get; set;}
    }
}