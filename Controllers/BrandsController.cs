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
    private string BaseUrl => $"{Request.Scheme}://{Request.Host}";
    [HttpGet]
    public async Task<IActionResult> GetBrands()
    {
        var brands = await _context.Brands
        .Select(b => new BrandDto
        {
            Id = b.Id,
            Name = b.Name,
            BrandImageUrl = b.ImagePath != null ? $"{BaseUrl}/brand-images/{b.ImagePath}" : null
        }
        )
        .ToListAsync();

        return Ok(brands);
        // var brands = await _context.Brands.ToListAsync();
        // return Ok(brands);
    }

}