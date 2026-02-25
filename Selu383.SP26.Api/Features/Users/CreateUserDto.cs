using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations;


namespace Selu383.SP26.Api.Features.Users;

public class CreateUserDto
{
    [Required]
    [MaxLength(64)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    public string[] Roles { get; set; } = Array.Empty<string>();
}