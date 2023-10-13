using System.ComponentModel.DataAnnotations;

namespace DotnetAuthYetAnother.Api.Models.Dtos;


public class TokenRequest
{
    [Required]
    public string Token { get; set; } = string.Empty;
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}