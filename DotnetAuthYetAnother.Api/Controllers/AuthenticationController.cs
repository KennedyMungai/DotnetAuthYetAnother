using DotnetAuthYetAnother.Api.Configuration;
using DotnetAuthYetAnother.Api.Models;
using DotnetAuthYetAnother.Api.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        // TODO: Generate the user tokens
    }
}