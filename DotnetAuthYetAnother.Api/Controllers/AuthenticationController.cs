using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
    private readonly IConfiguration _config;
    private readonly UserManager<IdentityUser> _userManager;

    public AuthenticationController(UserManager<IdentityUser> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    [HttpPost("Register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    [HttpPost("Login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] UserLoginRequestDto requestDto)
    {
        if (!ModelState.IsValid)
        {
            return await Task.FromResult(BadRequest(new AuthResults()
            {
                Errors = new List<string>()
                {
                    "Invalid Credentials"
                },
                Result = false
            }));
        }

        var userExists = await _userManager.FindByEmailAsync(requestDto.Email);

        if (userExists is null)
        {
            return await Task.FromResult(Unauthorized(new AuthResults()
            {
                Errors = new List<string>()
                {
                    "Invalid credentials"
                },
                Result = false
            }));
        }

        var isCorrectPassword = BCrypt.Net.BCrypt.Verify(requestDto.Password, userExists.PasswordHash);

        if (isCorrectPassword is false)
        {
            return await Task.FromResult(Unauthorized(new AuthResults()
            {
                Errors = new List<string>()
                {
                    "Invalid Credentials"
                },
                Result = false
            }));
        }

        var jwtToken = GenerateJwtToken(userExists);

        return await Task.FromResult(Ok(new AuthResults()
        {
            Token = jwtToken,
            Result = true
        }));
    }

    private string GenerateJwtToken(IdentityUser user)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.UTF8.GetBytes(_config.GetSection("JwtConfig:Secret").Value);

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