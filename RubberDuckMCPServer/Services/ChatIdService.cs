using System.Collections.Concurrent;
using RubberDuckMCPServer.Models.POCO;

namespace RubberDuckMCPServer.Services;

public static class ChatIdService
{
    private static readonly ConcurrentDictionary<Guid, byte> ChatIds = [];

    public static void LoadData(IEnumerable<Guid> chatIds)
    {
        foreach (var chatId in chatIds) ChatIds.TryAdd(chatId, 0);
    }
    
    public static bool IsUniqueChatId(Guid chatId) => !ChatIds.ContainsKey(chatId);
    
    public static bool Register(Guid chatId) => ChatIds.TryAdd(chatId, 0);
    
    public static bool DeleteChatId(Guid chatId) => ChatIds.TryRemove(chatId, out _);
}