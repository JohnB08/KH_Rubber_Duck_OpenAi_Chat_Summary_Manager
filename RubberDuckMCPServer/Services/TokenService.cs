namespace RubberDuckMCPServer.Services;

public static class TokenService
{
    public static string Token { get; set; } = string.Empty;
    public static bool ValidateToken(string token) => string.Equals(token, Token);
}