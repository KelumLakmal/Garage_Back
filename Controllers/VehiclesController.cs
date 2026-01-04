using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public VehiclesController(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
    }
    // [HttpGet]
    // public async Task<IActionResult> GetVehicles()
    // {
    //     var vehicles = await _context.Vehicles.Where(v => v.IsActive == true)
    //     .Include(v => v.Brand)
    //     .Include(v => v.Customer)
    //     .OrderByDescending(v => v.Id)
    //     .ToListAsync();

    //     return Ok(vehicles);
    // }
    [HttpGet]
    public async Task<IActionResult> GetVehicles()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var vehicles = await _context.Vehicles
        .Include(v => v.Brand)
        .Include(v => v.Customer)
        .OrderByDescending(v => v.Id)
        .Where(v => v.IsActive == true)
        .Select(v => new VehicleDto
        {
            Id = v.Id,
            PlateNumber = v.PlateNumber,
            Model = v.Model,
            ImageUrl = v.ImagePath != null ? $"{baseUrl}/vehicle-images/{v.ImagePath}" : null,
            Brand = new BrandDto
            {
                Id = v.Brand.Id,
                Name = v.Brand.Name
            },
            Customer = new CustomerDto
            {
                Id = v.Customer.Id,
                Name = v.Customer.Name,
                Mobile = v.Customer.Mobile,
                Nic = v.Customer.Nic,
                Email = v.Customer.Email
            }
        }
        ).ToListAsync();

        return Ok(vehicles);

        // var vehicles = await _context.Vehicles.Where(v => v.IsActive == true)
        // .Include(v => v.Brand)
        // .Include(v => v.Customer)
        // .OrderByDescending(v => v.Id)
        // .ToListAsync();

        // return Ok(vehicles);
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
    public async Task<IActionResult> SaveVehicle([FromForm] VehicleAddDto vehicleAddDto)
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

        string? imagePath = null;

        if (vehicleAddDto.Image != null)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "vehicle-images");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(vehicleAddDto.Image.FileName)}";
            System.Console.WriteLine(fileName);

            var fullPath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await vehicleAddDto.Image.CopyToAsync(stream);
            }
            imagePath = fileName; // store only filename in DB
        }

        var newVehicle = new Vehicle()
        {
            PlateNumber = vehicleAddDto.PlateNumber,
            Model = vehicleAddDto.Model,
            BrandId = vehicleAddDto.BrandId,
            CustomerId = vehicleAddDto.CustomerId,
            CreatedDate = DateTime.Now,
            ImagePath = imagePath
        };
        _context.Vehicles.Add(newVehicle);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetVehicleById), new { id = newVehicle.Id }, newVehicle);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateVehicle(int id, [FromForm] VehicleUpdateDto vehicleUpdateDto)
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

        // string? newImagePath = null;

        if (vehicleUpdateDto.Image != null)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "vehicle-images");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            if (vehicle.ImagePath != null)
            {
                var oldImagePath = Path.Combine(uploadsFolder, vehicle.ImagePath);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            var newFileName = $"{Guid.NewGuid()}{Path.GetExtension(vehicleUpdateDto.Image.FileName)}";

            var newFullPath = Path.Combine(uploadsFolder, newFileName);

            using (var stream = new FileStream(newFullPath, FileMode.Create))
            {
                await vehicleUpdateDto.Image.CopyToAsync(stream);
            }
            vehicle.ImagePath = newFileName;
        }
        else
        {
            // if (!string.IsNullOrEmpty(vehicle.ImagePath))
            // {
            //     var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(),
            //     "wwwroot",
            //     "vehicle-images",
            //     vehicle.ImagePath
            //     );

            //     if (System.IO.File.Exists(oldImagePath))
            //     {
            //         System.IO.File.Delete(oldImagePath);
            //     }
            //     vehicle.ImagePath = null;
            // }
        }

        vehicle.PlateNumber = vehicleUpdateDto.PlateNumber;
        vehicle.Model = vehicleUpdateDto.Model;
        vehicle.BrandId = vehicleUpdateDto.BrandId;
        vehicle.CustomerId = vehicleUpdateDto.CustomerId;
        vehicle.ModifiedDate = DateTime.Now;
        // vehicle.ImagePath = newImagePath;
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
        // soft delete by setting IsActive = false;
        vehicle.IsActive = false;
        vehicle.ModifiedDate = DateTime.Now;
        await _context.SaveChangesAsync();
        return NoContent();
        // _context.Vehicles.Remove(vehicle);
        // await _context.SaveChangesAsync();
        // return NoContent();
    }
}