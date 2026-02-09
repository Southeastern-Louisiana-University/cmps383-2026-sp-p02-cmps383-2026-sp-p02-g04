using System.ComponentModel.DataAnnotations;

namespace Selu383.SP26.Api.Features.Authentication;

public class LoginDto
{
    [Required]
    [MinLength(1)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    public string Password { get; set; } = string.Empty;
}