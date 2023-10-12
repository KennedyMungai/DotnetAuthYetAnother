using DotnetAuthYetAnother.Api.Configuration;
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
}