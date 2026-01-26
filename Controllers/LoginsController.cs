using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("api/[controller]")]
public class LoginsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;

    private readonly PasswordHasher<User> _passwordHasher;
    public LoginsController(ApplicationDbContext applicationDbContext, IConfiguration configuration)
    {
        _context = applicationDbContext;
        _config = configuration;
        _passwordHasher = new PasswordHasher<User>();
    }

    private string GenerateJwt(User user, List<string> permissions)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)

        };

        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);

    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var user = await _context.Users
        .Include(u => u.UserPermissions)
        .ThenInclude(up => up.Permission)
        .FirstOrDefaultAsync(u => u.UserName == loginDto.UserName && u.IsActive == true);

        if (user == null)
        {
            return Unauthorized();
        }

        // if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        // {
        //     return Unauthorized();
        // }

        var result = _passwordHasher.VerifyHashedPassword(
            user: user,
            hashedPassword: user.PasswordHash!,
            providedPassword: loginDto.Password
        );

        // var permissionList = new List<string>();

        if (result == PasswordVerificationResult.Failed)
        {
            return Unauthorized("Invalid username or password");
        }

        // foreach (var userPermission in user.UserPermissions)
        // {
        //     permissionList.Add(userPermission.Permission.Code);
            
        // }

        var permissions = user.UserPermissions
        .Select(up => up.Permission.Code)
        .ToList();

        var token = GenerateJwt(user, permissions);

        // var tokenDetails = new
        // {
        //     Token = token,
        //     User = new
        //     {
        //         Id = user.Id,
        //         UserName = user.UserName,
        //         Permissions = permissions
        //     }
        // };

        var tokenDetails = new
        {
            Token = token,
            User = new
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                // UserImage = user.UserImagePath
            }
        };

        return Ok(tokenDetails);

    }

}