using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class RepairsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RepairsController(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetRepairs()
    {
        var repairs = await _context.Repairs
        .Where(r => r.IsActive == true)
        .Select(r => new RepairDto
        {
            Id = r.Id,
            Name = r.Name,
            RepairCategory = new RepairCategoryDto
            {
                Id = r.RepairCategory.Id,
                Name = r.RepairCategory.Name
            }
        }).ToListAsync();
        // var repairs = await _context.Repairs
        // .Where(r => r.IsActive == true)
        // .Include(r => r.RepairCategory)
        // .ToListAsync();

        return Ok(repairs);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetRepairById(int id)
    {
        var repair = await _context.Repairs
        .Where(r => r.Id == id)
        .Select(r => new RepairDto
        {
            Id = r.Id,
            Name = r.Name,
            RepairCategory = new RepairCategoryDto
            {
                Id = r.RepairCategory.Id,
                Name = r.RepairCategory.Name
            }
        }).FirstOrDefaultAsync();

        if (repair is null)
        {
            return NotFound("Repair Not Found");
        }

        return Ok(repair);
    }

    [HttpGet("by-category/{categoryId:int}")]
    public async Task<IActionResult> GetRepairsByCategoryId(int categoryId)
    {
        var isCategoryExists =  await _context.RepairCategories.AnyAsync(rc => rc.Id == categoryId);
        if (!isCategoryExists)
        {
            return NotFound("Invalid Repair Category");
        }
        var repairs = await _context.Repairs
        .Where(r => r.IsActive == true && r.RepairCategoryId == categoryId)
        .Select(r => new RepairDto
        {
            Id = r.Id,
            Name = r.Name,
            RepairCategory = new RepairCategoryDto
            {
                Id = r.RepairCategory.Id,
                Name = r.RepairCategory.Name
            }
        })
        .ToListAsync();

        return Ok(repairs);

    }

}