using Selu383.SP26.Api.Features.Users;
using Selu383.SP26.Api.Features.Roles;
using Microsoft.AspNetCore.Identity;

namespace Selu383.SP26.Api.Features.UserRoles;

//must inherit from IdentityUserRole<int>
public class UserRole : IdentityUserRole<int>
{
    // set equal to default to avoid any null issues
    //needed since user and role are "required" properties 
    // creates an sql query that joins the user and role tables when querying for user roles
    public virtual User User { get; set; } = default!;
    public virtual Role Role { get; set; } = default!;
}

