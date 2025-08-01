using System.ComponentModel.DataAnnotations;

namespace RubberDuckMCPServer.Models.POCO;

public class Token
{
    [Key]
    public int Id { get; set; }
    public string OpenAiToken { get; set; }
}