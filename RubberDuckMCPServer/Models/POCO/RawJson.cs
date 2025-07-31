using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RubberDuckMCPServer.Models.POCO;

public class RawJson
{
    [Key]
    public int Id { get; set; }
    [ForeignKey(nameof(ChatId))]
    public int UniqueChatId { get; set; }
    public string JsonBlob { get; set; }
    public ChatId ChatId { get; set; }
}