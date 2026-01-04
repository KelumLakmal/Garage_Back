using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class BrandsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public BrandsController(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
    }
    [HttpGet]
    public async Task<IActionResult> GetBrands()
    {
        var brands = await _context.Brands.ToListAsync();
        return Ok(brands);
    }

}