using System.ComponentModel.DataAnnotations;

namespace RubberDuckMCPServer.Models.POCO;

public class ChatId
{
    [Key]
    public int Id { get; set; }
    public Guid GeneratedChatId { get; set; }
}