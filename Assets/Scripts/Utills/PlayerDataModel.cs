using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("player_data")]
public class PlayerDataModel : BaseModel
{
    [PrimaryKey("id")] public string Id { get; set; }
    [Column("data")] public PlayerData Data { get; set; }
}