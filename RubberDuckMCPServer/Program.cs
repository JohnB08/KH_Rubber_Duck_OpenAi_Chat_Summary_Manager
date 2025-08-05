
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RubberDuckMCPServer.Models.Context;
using RubberDuckMCPServer.Models.DTO;
using RubberDuckMCPServer.Models.POCO;
using RubberDuckMCPServer.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("./Log-.txt", rollingInterval:RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

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
    var logger =  scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    do
    {
        try
        {
            var chatIds = dbConnection.ChatIds.Select(c => c.GeneratedChatId);
            ChatIdService.LoadData(chatIds);
            var token = dbConnection.Tokens.FirstOrDefault();
            if (token is null)
            {
                var tokendata = SHA256.HashData(Guid.NewGuid().ToByteArray());
                var stringBuilder = new StringBuilder();
                foreach (var data in tokendata)
                {
                    stringBuilder.Append(data.ToString("x2"));
                }
                var tokenString = stringBuilder.ToString();
                await dbConnection.Tokens.AddAsync(new Token()
                {
                    OpenAiToken = tokenString,
                });
                await dbConnection.SaveChangesAsync();
                TokenService.Token = tokenString;
            }
            else TokenService.Token = token.OpenAiToken;
            logger.LogInformation("Token: {S}", TokenService.Token);
        }
        catch (SqliteException ex)
        {
        
            await dbConnection.Database.MigrateAsync();
        }
    } while (TokenService.Token ==  string.Empty);
    
}

app.Use(async (context, next) =>
{
    var authHeader = context.Request.Headers.Authorization;
    if (authHeader != $"Bearer {TokenService.Token}")
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Unauthorized");
        return;
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

app.MapGet("/chat_history", async (Guid id, int amount, McpDbContext context) =>
{
    return Results.Ok(await context.Chats.Include(c => c.ChatId).Where(c => c.ChatId.GeneratedChatId == id)
        .OrderByDescending(c => c.Id).Take(amount).ToListAsync());
});

app.Run();
