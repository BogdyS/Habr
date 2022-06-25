namespace Habr.Common.DTO;

public class TokenResponse
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}