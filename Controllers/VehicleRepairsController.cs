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
                Name = vr.Repair.Name,
                RepairCategory = new RepairCategoryDto
                {
                    Id = vr.Repair.RepairCategory.Id,
                    Name = vr.Repair.RepairCategory.Name
                }
            },
            RepairedDate = vr.CreatedDate,
            SpeedoMeter = vr.SpeedoMeter,
            Note = vr.Note
        })
        .OrderByDescending(vr => vr.Id)
        .ToListAsync();

        return Ok(vehicleRepairs);
    }

    [HttpPost]
    public async Task<IActionResult> SaveVehicleRepairs([FromBody] VehicleRepairAddDto vehicleRepairAddDto)
    {
        var isExists = await _context.Vehicles.AnyAsync(v => v.Id == vehicleRepairAddDto.VehicleId);

        if (!isExists)
        {
            return BadRequest("Invalid Vehicle");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        foreach (var repair in vehicleRepairAddDto.RepairsInfos)
        {
            var vehicleRepair = new VehicleRepair
            {
                VehicleId = vehicleRepairAddDto.VehicleId,
                RepairId = repair.RepairId,
                CreatedDate = DateTime.Now,
                IsActive = true,
                SpeedoMeter = vehicleRepairAddDto.SpeedoMeter,
                Note = repair.Note

            };
            _context.VehicleRepairs.Add(vehicleRepair);
        }
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateVehicleRepair(int id, [FromBody] VehicleRepairUpdateDto vehicleRepairUpdateDto)
    {
        var vehicleRepair = await _context.VehicleRepairs.FindAsync(id);

        if (vehicleRepair is null)
        {
            return NotFound();
        }

        var isVehicleExists = await _context.Vehicles.AnyAsync(v => v.Id == vehicleRepairUpdateDto.VehicleId);
        var isRepairExits = await _context.Repairs.AnyAsync(r => r.Id == vehicleRepairUpdateDto.RepairId);
 
        if (!isVehicleExists || !isRepairExits)
        {
            return BadRequest();
        }

        vehicleRepair.VehicleId = vehicleRepairUpdateDto.VehicleId;
        vehicleRepair.RepairId = vehicleRepairUpdateDto.RepairId;
        vehicleRepair.SpeedoMeter = vehicleRepairUpdateDto.SpeedoMeter;
        vehicleRepair.Note =vehicleRepairUpdateDto.Note;
        vehicleRepair.ModifiedDate = DateTime.Now;

        await _context.SaveChangesAsync();
        return Ok(vehicleRepair);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteVehicleRepair(int id)
    {
        var vehicleRepair = await _context.VehicleRepairs.FindAsync(id);

        if (vehicleRepair is null)
        {
            return NotFound();
        }

        vehicleRepair.IsActive = false;
        vehicleRepair.ModifiedDate = DateTime.Now;
        await _context.SaveChangesAsync();
        return NoContent();
    }

}