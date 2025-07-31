using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RubberDuckMCPServer.Models.Context;
using RubberDuckMCPServer.Models.DTO;
using RubberDuckMCPServer.Models.POCO;
using RubberDuckMCPServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<McpDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    var dbConnection = scope.ServiceProvider.GetRequiredService<McpDbContext>();
    try
    {
        var chatIds = dbConnection.ChatIds.Select(c => c.GeneratedChatId);
        ChatIdService.LoadData(chatIds);
    }
    catch (SqliteException ex)
    {
        
        await dbConnection.Database.MigrateAsync();
    }
}

var expected = builder.Configuration["ApiKey"];

app.Use(async (context, next) =>
{
    var authHeader = context.Request.Headers.Authorization;
    if (authHeader != $"Bearer {expected}")
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Unauthorized");
    }

    await next();
});

app.MapHealthChecks("/health");

app.MapGet("/chat_id",async (McpDbContext context) =>
{
    Guid newId;
    do
    {
        newId = Guid.NewGuid();
    } while (!ChatIdService.IsUniqueChatId(newId));

    ChatIdService.Register(newId);
    await context.ChatIds.AddAsync(new ChatId()
    {
        GeneratedChatId = newId
    });
    await context.SaveChangesAsync();
    return new {chatId = newId};
});

app.MapPost("/chat_summary", async (ActionRequestBody jsonRequest, McpDbContext context) =>
{
    var chatId = await context.ChatIds.Where(c => c.GeneratedChatId == jsonRequest.chatId).FirstOrDefaultAsync();
    if (chatId is null) return Results.Unauthorized();
    var existingRawJson = await context.RawJsons.Include(c=> c.ChatId).Where(c => c.ChatId.Equals(chatId)).FirstOrDefaultAsync();
    if (existingRawJson is null)
        await context.RawJsons.AddAsync(new RawJson()
        {
            ChatId = chatId,
            JsonBlob = JsonSerializer.Serialize(jsonRequest)
        });
    else existingRawJson.JsonBlob = JsonSerializer.Serialize(jsonRequest);
    await context.Chats.AddAsync(new Chat()
    {
        ChatId = chatId,
        Summary = jsonRequest.summary,
        TimeStamp = jsonRequest.timestamp,
    });
    await context.SaveChangesAsync();
    return Results.Ok();
});

app.MapGet("/chat_summary", async (Guid id, McpDbContext context) =>
{
    var rawJson = await context.RawJsons.Include(r => r.ChatId).Where(r => r.ChatId.GeneratedChatId == id)
        .FirstOrDefaultAsync();
    return rawJson is null ? Results.NotFound() : Results.Ok(JsonSerializer.Deserialize<ActionRequestBody>(rawJson.JsonBlob));
});

app.MapGet("/chat_history", async (Guid id, McpDbContext context) =>
{
    return Results.Ok(await context.Chats.Include(c => c.ChatId).Where(c => c.ChatId.GeneratedChatId == id)
        .OrderByDescending(c => c.Id).Take(10).ToListAsync());
});

app.Run();
