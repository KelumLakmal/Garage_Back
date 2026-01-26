using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class VehicleRepairsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public VehicleRepairsController(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetVehicleRepairs()
    {
        // var vehicleRepairs = await _context.VehicleRepairs
        // .Where(vr => vr.IsActive == true)
        // .Include(vr => vr.Vehicle)
        // .Include(vr => vr.Repair)
        // .OrderByDescending(vr => vr.Id)
        // .ToListAsync();
        var vehicleRepairs = await _context.VehicleRepairs
        .Where(vr => vr.IsActive == true)
        .Select(vr => new VehicleRepairDto
        {
            Id = vr.Id,
            Vehicle = new VehicleDto
            {
                Id = vr.Vehicle.Id,
                PlateNumber = vr.Vehicle.PlateNumber
            },
            Repair = new RepairDto
            {
                Id = vr.Repair.Id,
                Name = vr.Repair.Name
            },
            RepairedDate = vr.CreatedDate,
            SpeedoMeter = vr.SpeedoMeter
        })
        .OrderByDescending(vr => vr.Id)
        .ToListAsync();

        return Ok(vehicleRepairs);
    }
    
}