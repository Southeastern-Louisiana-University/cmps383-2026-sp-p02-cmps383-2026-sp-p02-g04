using System.ComponentModel.DataAnnotations;

namespace Selu383.SP26.Api.Features.Authentication;

public class LoginDto
{
    [Required]
    [MaxLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;
}