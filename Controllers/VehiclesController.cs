using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public VehiclesController(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
    }
    [HttpGet]
    public async Task<IActionResult> GetVehicles()
    {
        var vehicles = await _context.Vehicles
        .Include(v => v.Brand)
        .Include(v => v.Customer)
        .OrderByDescending(v => v.Id)
        .ToListAsync();

        return Ok(vehicles);
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetVehicleById(int id)
    {
        var vehicle = await _context.Vehicles
        .Include(v => v.Brand)
        .Include(v => v.Customer)
        .FirstOrDefaultAsync(v => v.Id == id);
        if (vehicle is null)
        {
            return NotFound();
        }
        return Ok(vehicle);
    }
    [HttpPost]
    public async Task<IActionResult> SaveVehicle([FromBody] VehicleAddDto vehicleAddDto)
    {
        var brandExists = await _context.Brands.AnyAsync(b => b.Id == vehicleAddDto.BrandId);
        if (!brandExists)
        {
            return BadRequest("Invalid BrandId.");
        }
        var customerExists = await _context.Customers.AnyAsync(c => c.Id == vehicleAddDto.CustomerId);
        if (!customerExists)
        {
            return BadRequest("Invalid Customer");
        }
        var newVehicle = new Vehicle()
        {
            PlateNumber = vehicleAddDto.PlateNumber,
            Model = vehicleAddDto.Model,
            BrandId = vehicleAddDto.BrandId,
            CustomerId = vehicleAddDto.CustomerId
        };
        _context.Vehicles.Add(newVehicle);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetVehicleById), new { id = newVehicle.Id }, newVehicle);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateVehicle(int id, [FromBody] VehicleUpdateDto vehicleUpdateDto)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle is null)
        {
            return NotFound();
        }
        var brandExists = await _context.Brands.AnyAsync(b => b.Id == vehicleUpdateDto.BrandId);
        if (!brandExists)
        {
            return BadRequest("Invalid BrandId.");
        }

        var customerExists = await _context.Customers.AnyAsync(c => c.Id == vehicleUpdateDto.CustomerId);
        if (!customerExists)
        {
            return BadRequest("Invalid Customer");
        }
        vehicle.PlateNumber = vehicleUpdateDto.PlateNumber;
        vehicle.Model = vehicleUpdateDto.Model;
        vehicle.BrandId = vehicleUpdateDto.BrandId;
        vehicle.CustomerId = vehicleUpdateDto.CustomerId;
        await _context.SaveChangesAsync();
        return Ok(vehicle);
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteVehicle(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle is null)
        {
            return NotFound();
        }
        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}