
namespace Selu383.SP26.Api.Features.Locations;

//FOR OUPUT!!
//since the phase 2 instruction state that we can't send or receive
// entity types only dto types 
// need seperate dto tyoes for reading "get" vs creating "post" "put"
public class LocationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int TableCount { get; set; }
    public int? ManagerId { get; set; }

}
