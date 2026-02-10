using System.ComponentModel.DataAnnotations;

namespace Selu383.SP26.Api.Features.Locations;

//FOR INPUT!!
// allows "post " and "put"
public class LocationCreateDto
{
    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int TableCount { get; set; }

    public int? ManagerId { get; set; }

}