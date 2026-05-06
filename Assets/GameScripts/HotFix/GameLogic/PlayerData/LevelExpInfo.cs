namespace GameLogic
{
    public class LevelExpInfo
    {
        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; set;}
        
        /// <summary>
        /// 经验
        /// </summary>
        public int Exp { get; set;}

        public void Reset()
        {
            Level = 0;
            Exp = 0;
        }
    }
}