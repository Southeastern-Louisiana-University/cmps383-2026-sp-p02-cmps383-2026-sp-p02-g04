using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Selu383.SP26.Api.Features.Authentication;
using Selu383.SP26.Api.Features.Users;

namespace Selu383.SP26.Api.Controllers;

[Route("api/authentication")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    //manager to create and find users -constructor
    public AuthenticationController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    //login method to check if the user exists and if the password is correct
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto dto)
    {
        //find the user in db by username
        var user = await _userManager.FindByNameAsync(dto.UserName);
        if (user == null)
        {
            //if the user doen't exist return bad request
            return BadRequest();
    }
        //password checker
        //false to ensure the user isn't locked out after failed attempts
        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
        {
            //wrong password
            return BadRequest();

        }
        //Auth cookie so the user stays logged in
        await _signInManager.SignInAsync(user, false);
        //return user info using UserDto and get the roles of the user
        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new UserDto
        {
            Id = user.Id,
            UserName = user.UserName!,//added the ! to tell the compiler everything is fine this isn't null
            Roles = roles.ToArray()
        });
    }
    [HttpGet("me")]
    [Authorize] //user must be logged in to get to this
    public async Task<ActionResult<UserDto>> Me()
    {
        //gets usersname from the auth cookie
        var username = User.Identity?.Name;
        if (username == null)
        {
            return Unauthorized();//if the user isn't logged in return unauthorized
        }
        //grabs the user from db and returns user info using UserDto
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
        {
            return Unauthorized(); //if the user isn't found return unauthorized

        }
        //returns the user dto
        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new UserDto
        {
            Id = user.Id,
            UserName = user.UserName!,
            Roles = roles.ToArray()
        });
    }
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok ();
    }
}