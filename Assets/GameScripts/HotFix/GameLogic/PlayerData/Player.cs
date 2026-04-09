namespace GameLogic
{
    public class Player
    {
        public static readonly Player Self = new();
        
        public PlayerSession Session { get; } = new();
        public PlayerData Data { get; } = new();
    }
}