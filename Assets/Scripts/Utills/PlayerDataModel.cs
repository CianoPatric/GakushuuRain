using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

[Table("player_data")]
public class PlayerDataModel : BaseModel
{
    [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
    [PrimaryKey("id", false)] public string Id { get; set; }
    [Column("user_id")] public string UserId { get; set; }
    [Column("slot_index")] public int SlotIndex { get; set; }
    [Column("data")] public PlayerData Data { get; set; }
}