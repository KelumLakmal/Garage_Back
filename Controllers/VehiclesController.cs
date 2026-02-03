using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
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

    private string BaseUrl => $"{Request.Scheme}://{Request.Host}";
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
    [Authorize(Policy = "VEHICLE_VIEW")]
    [HttpGet]
    public async Task<IActionResult> GetVehicles([FromQuery] VehicleFilterDto vehicleFilterDto)
    {
        // var baseUrl = $"{Request.Scheme}://{Request.Host}";

        IQueryable<Vehicle> query = _context.Vehicles.Where(v => v.IsActive == true);

        if (!string.IsNullOrWhiteSpace(vehicleFilterDto.PlateNumber))
        {
            query = query.Where(v => EF.Functions.Like(v.PlateNumber, $"%{vehicleFilterDto.PlateNumber}%"));
        }
        if (vehicleFilterDto.BrandId > 0)
        {
            query = query.Where(v => v.BrandId == vehicleFilterDto.BrandId);
        }
        if (vehicleFilterDto.CustomerId > 0)
        {
            query = query.Where(v => v.CustomerId == vehicleFilterDto.CustomerId);
        }

        var vehicles = await query
       //    .Include(v => v.Brand)
       //    .Include(v => v.Customer)
       .OrderByDescending(v => v.Id)
       .Select(v => new VehicleDto
       {
           Id = v.Id,
           PlateNumber = v.PlateNumber,
           Model = v.Model,
           ImageUrl = v.ImagePath != null ? $"{BaseUrl}/vehicle-images/{v.ImagePath}" : null,
           Brand = v.Brand == null ? null : new BrandDto
           {
               Id = v.Brand.Id,
               Name = v.Brand.Name,
               BrandImageUrl = v.Brand.ImagePath != null ? $"{BaseUrl}/brand-images/{v.Brand.ImagePath}" : null
           },
           Customer = v.Customer == null ? null : new CustomerDto
           {
               Id = v.Customer.Id,
               Name = v.Customer.Name,
               Mobile = v.Customer.Mobile,
               Nic = v.Customer.Nic,
               Email = v.Customer.Email
           }
       })
       .ToListAsync();
        return Ok(vehicles);




        // var vehicles = await _context.Vehicles
        // .Include(v => v.Brand)
        // .Include(v => v.Customer)
        // .OrderByDescending(v => v.Id)
        // .Where(v => v.IsActive == true)
        // .Select(v => new VehicleDto
        // {
        //     Id = v.Id,
        //     PlateNumber = v.PlateNumber,
        //     Model = v.Model,
        //     ImageUrl = v.ImagePath != null ? $"{baseUrl}/vehicle-images/{v.ImagePath}" : null,
        //     Brand = new BrandDto
        //     {
        //         Id = v.Brand.Id,
        //         Name = v.Brand.Name
        //     },
        //     Customer = new CustomerDto
        //     {
        //         Id = v.Customer.Id,
        //         Name = v.Customer.Name,
        //         Mobile = v.Customer.Mobile,
        //         Nic = v.Customer.Nic,
        //         Email = v.Customer.Email
        //     }
        // }
        // ).ToListAsync();

        // return Ok(vehicles);

        // var vehicles = await _context.Vehicles.Where(v => v.IsActive == true)
        // .Include(v => v.Brand)
        // .Include(v => v.Customer)
        // .OrderByDescending(v => v.Id)
        // .ToListAsync();

        // return Ok(vehicles);
    }
    // [HttpGet("{id:int}")]
    // public async Task<IActionResult> GetVehicleById(int id)
    // {
    //     var vehicle = await _context.Vehicles
    //     .Include(v => v.Brand)
    //     .Include(v => v.Customer)
    //     .FirstOrDefaultAsync(v => v.Id == id);
    //     if (vehicle is null)
    //     {
    //         return NotFound();
    //     }
    //     return Ok(vehicle);
    // }
    [Authorize(Policy = "VEHICLE_VIEW")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetVehicleById(int id)
    {
        // var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var vehicle = await _context.Vehicles
        .Where(v => v.Id == id)
        .Select(v => new VehicleDto
        {
            Id = v.Id,
            PlateNumber = v.PlateNumber,
            Model = v.Model,
            ImageUrl = v.ImagePath != null ? $"{BaseUrl}/vehicle-images/{v.ImagePath}" : null,
            Brand = v.Brand == null ? null : new BrandDto
            {
                Id = v.Brand.Id,
                Name = v.Brand.Name
            },
            Customer = v.Customer == null ? null : new CustomerDto
            {
                Id = v.Customer.Id,
                Name = v.Customer.Name,
                Mobile = v.Customer.Mobile,
                Nic = v.Customer.Nic,
                Email = v.Customer.Email
            }
        }
        )
        .FirstOrDefaultAsync();

        if (vehicle is null)
        {
            return NotFound();
        }

        return Ok(vehicle);





        // var vehicle = await _context.Vehicles.FindAsync(id);
        // if (vehicle is null)
        // {
        //     return NotFound();
        // }

        // var vehicleById = new VehicleDto
        // {
        //     Id = vehicle.Id,
        //     PlateNumber = vehicle.PlateNumber,
        //     Model = vehicle.Model,
        //     ImageUrl = vehicle.ImagePath != null ? $"{baseUrl}/vehicle-images/{vehicle.ImagePath}" : null,
        //     Brand = vehicle.Brand == null ? null : new BrandDto
        //     {
        //         Id = vehicle.Brand.Id,
        //         Name = vehicle.Brand.Name
        //     },
        //     Customer = vehicle.Customer == null ? null : new CustomerDto
        //     {
        //         Id = vehicle.Customer.Id,
        //         Name = vehicle.Customer.Name,
        //         Mobile = vehicle.Customer.Mobile,
        //         Nic = vehicle.Customer.Nic,
        //         Email = vehicle.Customer.Email
        //     } 
        // };
        // return Ok(vehicleById);

    }

    [Authorize(Policy = "VEHICLE_CREATE")]
    [HttpPost]
    public async Task<IActionResult> SaveVehicle([FromForm] VehicleAddDto vehicleAddDto)
    {
        if (string.IsNullOrWhiteSpace(vehicleAddDto.PlateNumber))
        {
            return BadRequest("PlateNumber is required..");
        }
        if (string.IsNullOrWhiteSpace(vehicleAddDto.Model))
        {
            return BadRequest("Model is required..");
        }
        var plateNo = vehicleAddDto.PlateNumber.Trim().ToLower();

        var isPlateNoExists = await _context.Vehicles.AnyAsync(v =>
            v.IsActive == true &&
            v.PlateNumber != null &&
            string.Equals(v.PlateNumber, plateNo));

        if (isPlateNoExists)
        {
            return BadRequest("PlateNumber is already exists..");
        }

        var brandExists = await _context.Brands.AnyAsync(b => b.Id == vehicleAddDto.BrandId);
        if (!brandExists)
        {
            return BadRequest("Invalid BrandId.");
        }
        var customerExists = await _context.Customers.AnyAsync(c => c.IsActive == true && c.Id == vehicleAddDto.CustomerId);
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

    [Authorize(Policy = "VEHICLE_UPDATE")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateVehicle(int id, [FromForm] VehicleUpdateDto vehicleUpdateDto)
    {
        if (string.IsNullOrWhiteSpace(vehicleUpdateDto.PlateNumber))
        {
            return BadRequest("PlateNumber is required..");
        }
        if (string.IsNullOrWhiteSpace(vehicleUpdateDto.Model))
        {
            return BadRequest("Model is required..");
        }
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

        var customerExists = await _context.Customers.AnyAsync(c => c.IsActive == true && c.Id == vehicleUpdateDto.CustomerId);
        if (!customerExists)
        {
            return BadRequest("Invalid Customer");
        }

        var plateNo = vehicleUpdateDto.PlateNumber.Trim().ToLower();

        var isPlateNoExists = await _context.Vehicles.AnyAsync(v =>
            v.Id != vehicle.Id &&
            v.PlateNumber != null &&
            string.Equals(v.PlateNumber, plateNo));
        
        if (isPlateNoExists)
        {
            return BadRequest("PlateNumber is already exists..");
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

        vehicle.PlateNumber = vehicleUpdateDto.PlateNumber;
        vehicle.Model = vehicleUpdateDto.Model;
        vehicle.BrandId = vehicleUpdateDto.BrandId;
        vehicle.CustomerId = vehicleUpdateDto.CustomerId;
        vehicle.ModifiedDate = DateTime.Now;
        // vehicle.ImagePath = newImagePath;
        await _context.SaveChangesAsync();
        return Ok(vehicle);
    }

    [Authorize(Policy = "VEHICLE_DELETE")]
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