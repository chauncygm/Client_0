namespace GameMain.Scripts.Logic.Player.Data
{
    public class Player
    {
        public static readonly Player Self = new();
        public PlayerSession Session { get; set;}
        
        public PlayerData Data { get; set; }
        
        public Player()
        {
            Session = new PlayerSession();
            Data = new PlayerData();
        }
    }
}