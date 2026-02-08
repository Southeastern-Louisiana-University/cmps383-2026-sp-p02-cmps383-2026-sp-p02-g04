using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Selu383.SP26.Api.Features.Users;
using System.Threading.Tasks;

namespace Selu383.SP26.Api.Features.Authentication;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public AuthenticationController(SignInManager<User> signInManager, UserManager<User> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(); 
        }
        return Ok(new UserDto
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty
        }); 
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.UserName);
        if (user == null)
        {
            return BadRequest("Invalid username or password");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
        {
            return BadRequest("Invalid username or password");
        }

        await _signInManager.SignInAsync(user, isPersistent: true);

        return Ok(new UserDto
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty
        });
    } // 

    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }
}