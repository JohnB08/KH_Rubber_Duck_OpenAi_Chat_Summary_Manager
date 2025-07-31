using Microsoft.EntityFrameworkCore;
using RubberDuckMCPServer.Models.POCO;

namespace RubberDuckMCPServer.Models.Context;

public class McpDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Chat> Chats { get; set; }
    public DbSet<RawJson> RawJsons { get; set; }
    public DbSet<ChatId> ChatIds { get; set; }
}