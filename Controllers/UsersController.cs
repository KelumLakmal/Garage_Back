using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;
    public UsersController(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
        _passwordHasher = new PasswordHasher<User>();
    }
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        // var users = await _context.Users
        // .Include(u => u.UserPermissions)
        // .ThenInclude(up => up.Permission)
        // .OrderByDescending(u => u.Id).ToListAsync();
        // return Ok(users);

        var users = await _context.Users
        .Where(u => u.IsActive == true)
        .Select(u => new UserDto
        {
            FirstName = u.FirstName,
            LastName = u.LastName,
            UserImagePath = u.UserImagePath
        }).ToListAsync();

        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> SaveUser(UserAddDto userAddDto)
    {
        var userExits = await _context.Users.AnyAsync(u => u.UserName == userAddDto.UserName);
        if (userExits)
        {
            return BadRequest("UserName Already Exists..");
        }
        var user = new User
        {
            UserName = userAddDto.UserName,
            FirstName = userAddDto.FirstName,
            LastName = userAddDto.LastName,
            CreatedDate = DateTime.Now,
            IsActive = true
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, userAddDto.Password);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return StatusCode(StatusCodes.Status201Created, null);
    }
}