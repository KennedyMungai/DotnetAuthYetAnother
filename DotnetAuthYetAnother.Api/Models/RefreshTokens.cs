namespace DotnetAuthYetAnother.Api.Models;


public class RefreshTokens
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string JwtToken { get; set; } = string.Empty;
    public string JwtId { get; set; } = string.Empty;
    public bool IsUsed { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime AddedDate { get; set; }
    public DateTime ExpiryDate { get; set; }
}