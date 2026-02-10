using Microsoft.AspNetCore.Identity;
using Selu383.SP26.Api.Features.UserRoles;

namespace Selu383.SP26.Api.Features.Roles;

//must inherit identityrole<int>
public class Role : IdentityRole<int> 
{
    // the middle table that connects users and roles, when UserRole is called, the database will be queried to fetch the rolw
    public virtual ICollection<UserRole> Users { get; set; } = new List<UserRole>();

}

