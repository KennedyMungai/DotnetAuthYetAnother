using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DotnetAuthYetAnother.Api.Configuration;
using DotnetAuthYetAnother.Api.Models;
using DotnetAuthYetAnother.Api.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAuthYetAnother.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly JwtConfig _config;

    public AuthenticationController(UserManager<IdentityUser> userManager, JwtConfig config)
    {
        _userManager = userManager;
        _config = config;
    }

    [HttpPost("{Register}")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto requestDto)
    {
        if (!ModelState.IsValid)
        {
            return await Task.FromResult(BadRequest(ModelState));
        }

        var userExists = await _userManager.FindByEmailAsync(requestDto.Email);

        if (userExists is not null)
        {
            return await Task.FromResult(BadRequest(new AuthResults()
            {
                Result = false,
                Errors = new List<string>()
                {
                    "The Email already exists"
                }
            }));
        }

        var newUser = new IdentityUser()
        {
            Email = requestDto.Email,
            UserName = requestDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(requestDto.Password)
        };

        var isCreated = await _userManager.CreateAsync(newUser);

        if (!isCreated.Succeeded)
        {
            return await Task.FromResult(BadRequest(new AuthResults()
            {
                Errors = isCreated.Errors.Select(x => x.Description).ToList(),
                Result = false
            }));
        }

        var token = GenerateJwtToken(newUser);

        return await Task.FromResult(Ok(new AuthResults()
        {
            Token = token,
            Result = true
        }));
    }

    private string GenerateJwtToken(IdentityUser user)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.UTF8.GetBytes(_config.Secret);

        // Token Descriptor
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new Claim[]{
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user!.Email),
                new Claim(JwtRegisteredClaimNames.Email, user!.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);

        var jwtToken = jwtTokenHandler.WriteToken(token);

        return jwtToken;
    }
}