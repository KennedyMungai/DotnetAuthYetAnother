namespace DotnetAuthYetAnother.Api.Models;


public class AuthResults
{
    public string Token { get; set; } = string.Empty;
    public bool Result { get; set; } = false;
    public List<string> Errors { get; set; } = new();
}