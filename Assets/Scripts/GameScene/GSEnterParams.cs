public class GSEnterParams 
{ 
    public PlayerData InitialPlayerData { get; } 
    
    public GSEnterParams(PlayerData initialPlayerData) 
    {
        InitialPlayerData = initialPlayerData;
    }
}