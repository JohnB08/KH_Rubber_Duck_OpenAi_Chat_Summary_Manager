using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RubberDuckMCPServer.Models.POCO;

public class Chat
{
    [Key]
    public int Id { get; set; }
    [ForeignKey(nameof(ChatId))]
    public int UniqueChatId { get; set; }
    public DateTime TimeStamp { get; set; }
    public string Summary { get; set; }
    public ChatId ChatId { get; set; }
}