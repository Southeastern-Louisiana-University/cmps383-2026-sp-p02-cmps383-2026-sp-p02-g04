using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Selu383.SP26.Api.Features.Users;
using Selu383.SP26.Api.Features.Roles;

namespace Selu383.SP26.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public UsersController(UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")] //only admin role can create a new user
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto dto)
    {
        //checks for duplicate user
        var existingUser = await _userManager.FindByNameAsync(dto.UserName);
        if (existingUser != null)
        {
            return BadRequest("Username already exists.");
        }
        //check for empty roles
        if (dto.Roles == null || dto.Roles.Length == 0)
        {
            return BadRequest("You must provide at least one role.");

        }
        //check if the role actually exists
        foreach (var roleName in dto.Roles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return BadRequest($"Role '{roleName}' does not exist.");
            }
        }
        //create the user
        var user = new User
        {
            UserName = dto.UserName,
        };
        //save user and password valid
        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            //return 400 error if password is weak
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);

            }
            return BadRequest(ModelState);

        }
        //assign roles
        await _userManager.AddToRolesAsync(user, dto.Roles);
        //return it being a success
        return Ok(new UserDto
        {
            Id = user.Id,
            UserName = user.UserName!,
            Roles = dto.Roles

        });
    }
}
        