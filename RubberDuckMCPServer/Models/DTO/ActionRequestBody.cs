using System.Text.Json.Serialization;

namespace RubberDuckMCPServer.Models.DTO;

public class ActionRequestBody
{
    [JsonPropertyName("chat_id")]
    public Guid chatId { get; set; }
    [JsonPropertyName("summary")]
    public string summary { get; set; }
    [JsonPropertyName("timestamp")]
    public DateTime timestamp { get; set; }
    
}