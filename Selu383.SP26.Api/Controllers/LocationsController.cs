using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Selu383.SP26.Api.Data;
using Selu383.SP26.Api.Features.Locations;
using System.Security.Claims; //used to find who is logged in 


namespace Selu383.SP26.Api.Controllers;

[Route("api/locations")]
[ApiController]
public class LocationsController(DataContext dataContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<LocationDto>>> GetAll()
    {
        // grab data
        var locations = await dataContext.Locations.ToListAsync();
        // map to dto
        var dtos = locations.Select(x => new LocationDto
        {
            Id = x.Id,
            Name = x.Name,
            Address = x.Address,
            ManagerId = x.ManagerId
        }).ToList();

        return Ok(dtos);
    }
        [HttpGet("{id}")]
        public async Task<ActionResult<LocationDto>> GetById(int id)
        {
            var location = await dataContext.Locations.FindAsync(id);

            if (location == null)
            {
                return NotFound();
            }

            return Ok(new LocationDto
            {
                Id = location.Id,
                Name = location.Name,
                Address = location.Address,
                TableCount = location.TableCount,
                ManagerId = location.ManagerId
            });
        }

    [HttpPost]
    [Authorize(Roles = "Admin")] //only allow admins to create locations
    public async Task<ActionResult<LocationDto>> Create(LocationCreateDto dto)
    {
        if (dto.ManagerId.HasValue)//validdate manager exista

        {
            var managerExists = await dataContext.Users.AnyAsync(u => u.Id == dto.ManagerId.Value);
            if (!managerExists)
            {
                return BadRequest("Invalid ManagerId");
            }
        }
        var location = new Location
        {
            Name = dto.Name,
            Address = dto.Address,
            TableCount = dto.TableCount,
            ManagerId = dto.ManagerId
        };

        dataContext.Locations.Add(location);
        await dataContext.SaveChangesAsync();

        var resultDto = new LocationDto
        {
            Id = location.Id,
            Name = location.Name,
            Address = location.Address,
            TableCount = location.TableCount,
            ManagerId = location.ManagerId
        };
        return CreatedAtAction(nameof(GetById), new { id = location.Id }, resultDto);
    }

    [HttpPut("{id}")]
    [Authorize] //has to be logged in
    public async Task<ActionResult<LocationDto>> Update(int id, LocationCreateDto dto)
    {
        var location = await dataContext.Locations.FindAsync(id);
        if (location == null) return NotFound();
        //check permissions
        //get logged in user id
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.IsInRole("Admin");
        var isManager = location.ManagerId == userId;

        //managers can only update their own location, admins can update any location
        if (!isAdmin && !isManager)
        {
            return Forbid();
        }
        //managers can't change the manager of the location only the admins can
        if (!isAdmin && dto.ManagerId != location.ManagerId)
        {
            return Forbid();
        }

        location.Name = dto.Name;
        location.Address = dto.Address;
        location.TableCount = dto.TableCount;

        //only update mangerId if the role is admin
        if (isAdmin)
        {
            location.ManagerId = dto.ManagerId;
        }
        await dataContext.SaveChangesAsync();

        return Ok(new LocationDto
        {
            Id = location.Id,
            Name = location.Name,
            Address = location.Address,
            TableCount = location.TableCount,
            ManagerId = location.ManagerId
        });
    }

                [HttpDelete("{id}")]
                [Authorize(Roles = "Admin")] //only allow admins to delete locations
                public async Task<ActionResult> Delete(int id)
                {
                    var location = await dataContext.Locations.FindAsync(id);
                    if (location == null) return NotFound();

                    dataContext.Locations.Remove(location);
                    await dataContext.SaveChangesAsync();

                    return Ok();

                }


            
        

    
}