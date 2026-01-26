using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class RepairCategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public RepairCategoriesController(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetRepairCategories()
    {
        var repairCategories = await _context.RepairCategories
        .Where(rc => rc.IsActive == true)
        .Select(rc => new RepairCategoryDto
        {
            Id = rc.Id,
            Name = rc.Name
        }).ToListAsync();

        return Ok(repairCategories);
    }
    
}