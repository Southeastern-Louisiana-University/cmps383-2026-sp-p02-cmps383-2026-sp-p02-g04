using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Selu383.SP26.Api.Data;
using Selu383.SP26.Api.Features.Locations;
using System.Security.Claims;
using Selu383.SP26.Api.Features.Roles;



namespace Selu383.SP26.Api.Controllers;

[Route("api/locations")]
[ApiController]
public class LocationsController : ControllerBase
{
    private readonly DataContext _dataContext;
    public LocationsController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LocationDto>>> GetAll()
    {
        // grab data
        var locations = await _dataContext.Locations.ToListAsync();
        // map to dto
        var dtos = locations.Select(x => new LocationDto
        {
            Id = x.Id,
            Name = x.Name,
            Address = x.Address,
            ManagerId = x.ManagerId,
            TableCount = x.TableCount
        });

        return Ok(dtos);
    }
        [HttpGet("{id}")]
        public async Task<ActionResult<LocationDto>> GetById(int id)
        {
            var location = await _dataContext.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }
        var dto = new LocationDto
        {
            Id = location.Id,
            Name = location.Name,
            Address = location.Address,
            ManagerId = location.ManagerId,
            TableCount = location.TableCount
        };

        return Ok(dto);
        }

    [HttpPost]
    [Authorize(Roles = "Admin")] //only allow admins to create locations
    public async Task<ActionResult<LocationDto>> Create(LocationDto dto)
    {
        //name too long check
        if (dto.Name.Length > 120 || dto.Address.Length > 120)
        {
            return BadRequest("The Name or Address must be 120 characters or less.");

        }
        //validation table count check; check if it's null or 0 or less
        if (dto.TableCount == null || dto.TableCount <= 0)
        {
            return BadRequest("The table count must be at least one.");
        }
        //mapping dto to entity
        var location = new Location
        {
            Name = dto.Name,
            Address = dto.Address,
            ManagerId = dto.ManagerId,
            TableCount = dto.TableCount.Value //extracts num from nullable
        };
        //save to db
        _dataContext.Locations.Add(location);
        await _dataContext.SaveChangesAsync();
        //return result
        dto.Id = location.Id;
        return CreatedAtAction(nameof(GetById), new { id = location.Id }, dto);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<LocationDto>> Update(int id, LocationDto dto)
    {
        //validation length check
        if (dto.Name.Length > 120 || dto.Address.Length > 120)
        {
            return BadRequest("The Name or Address must be 120 characters or less.");

        }
        //validation table count check; checks if it's null or 0 or less
        if (dto.TableCount == null || dto.TableCount <= 0) 
        {
            return BadRequest("TableCount must be at least 1.");
        }
        //find existing lo
        var location = await _dataContext.Locations.FindAsync(id);
        if (location == null)
        {
            return NotFound();
        }
        //admins can edit anyone, users can only edit if they are the manager
        if (!User.IsInRole("Admin"))
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (location.ManagerId != userId)
            {
                return Forbid();
            }
        }
            //properties
        location.Name = dto.Name;
        location.Address = dto.Address;
        location.ManagerId = dto.ManagerId;
        location.TableCount = dto.TableCount.Value;
        //save 
        await _dataContext.SaveChangesAsync();
        //return updated dto
        dto.Id = location.Id;
        return Ok(dto);
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")] //only allow admins to delete locations
    public async Task<ActionResult> Delete(int id)
    {
        var location = await _dataContext.Locations.FindAsync(id);
        if (location == null)
        {
            return NotFound();
        }

        _dataContext.Locations.Remove(location);
        await _dataContext.SaveChangesAsync();

        return Ok();

    }
    
}