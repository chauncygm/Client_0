namespace GameMain.Scripts.Logic.Player.Data
{
    public class Player
    {
        public static readonly Player Self = new();
        
        public PlayerSession Session { get; set;} = new();
        public PlayerData Data { get; set; } = new();
    }
}