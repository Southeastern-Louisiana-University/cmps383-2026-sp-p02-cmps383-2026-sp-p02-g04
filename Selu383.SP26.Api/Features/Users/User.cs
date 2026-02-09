using System.ComponentModel.DataAnnotations;
using Selu383.SP26.Api.Features.UserRoles; 
using Microsoft.AspNetCore.Identity;

namespace Selu383.SP26.Api.Features.Users;

//must inherit from IdentityUser
public class User : IdentityUser<int>
{
    // the middle table that connects users and roles, when UserRole is called, the database will be queried to fetch the user
    public virtual ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
}
