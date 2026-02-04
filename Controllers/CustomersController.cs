using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CustomersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // [HttpGet]
    // public async Task<IActionResult> GetCustomers()
    // {
    //     var allCustomers = await _context.Customers
    //     .OrderByDescending(c => c.Id)
    //     .ToListAsync();
    //     return Ok(allCustomers);
    // }
    [Authorize(Policy = "CUSTOMER_VIEW")]
    [HttpGet]
    public async Task<IActionResult> GetCustomers([FromQuery] CustomerFilterDto customerFilterDto)
    {
        IQueryable<Customer> query = _context.Customers.Where(c => c.IsActive == true);

        if (!string.IsNullOrWhiteSpace(customerFilterDto.Name))
        {
            query = query.Where(c => EF.Functions.Like(c.Name, $"%{customerFilterDto.Name}%"));
        }
        if (!string.IsNullOrWhiteSpace(customerFilterDto.Mobile))
        {
            query = query.Where(c => EF.Functions.Like(c.Mobile, $"%{customerFilterDto.Mobile}%"));
        }
        if (!string.IsNullOrWhiteSpace(customerFilterDto.Nic))
        {
            query = query.Where(c => EF.Functions.Like(c.Nic, $"%{customerFilterDto.Nic}%"));
        }
        if (!string.IsNullOrWhiteSpace(customerFilterDto.Email))
        {
            query = query.Where(c => EF.Functions.Like(c.Email, $"%{customerFilterDto.Email}%"));
        }

        var filteredCustomers = await query.OrderByDescending(c => c.Id)
        .ToListAsync();
        return Ok(filteredCustomers);

        // var allCustomers = await _context.Customers
        // .OrderByDescending(c => c.Id)
        // .ToListAsync();
        // return Ok(allCustomers);
    }

    [Authorize(Policy = "CUSTOMER_VIEW")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCustomerById(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer is null)
        {
            return NotFound();
        }
        return Ok(customer);
    }

    [Authorize(Policy = "CUSTOMER_CREATE")]
    [HttpPost]
    public async Task<IActionResult> SaveCustomer([FromBody] CustomerAddDto customerAddDto)
    {
        if (string.IsNullOrWhiteSpace(customerAddDto.Name))
        {
            return BadRequest("Name is required");
        }
        if (!string.IsNullOrWhiteSpace(customerAddDto.Nic))
        {
            var nicInIgnoreCase = customerAddDto.Nic.Trim().ToLower();
            var nicExists = await _context.Customers.AnyAsync(c => string.Equals(c.Nic, nicInIgnoreCase));

            if (nicExists)
            {
                return BadRequest("NIC already exists..");
            }

        }
        if (!string.IsNullOrWhiteSpace(customerAddDto.Mobile))
        {
            var mobileExists = await _context.Customers.AnyAsync(c => c.Mobile != null && c.Mobile == customerAddDto.Mobile);
            if (mobileExists)
            {
                return BadRequest("Mobile is already exists..");
            }
        }
        if (!string.IsNullOrWhiteSpace(customerAddDto.Email))
        {
            var email = customerAddDto.Email.Trim().ToLower();
            var emailExists = await _context.Customers.AnyAsync(c => c.Email != null && string.Equals(c.Email, email));
            if (emailExists)
            {
                return BadRequest("Email is already Exists..");
            }
        }

        var newCustomer = new Customer()
        {
            Name = customerAddDto.Name,
            Mobile = customerAddDto.Mobile,
            Nic = customerAddDto.Nic,
            Email = customerAddDto.Email
        };
        _context.Customers.Add(newCustomer);
        await _context.SaveChangesAsync();
        return StatusCode(StatusCodes.Status201Created, newCustomer);
    }

    [Authorize(Policy = "CUSTOMER_UPDATE")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerUpdateDto customerUpdateDto)
    {
        if (string.IsNullOrWhiteSpace(customerUpdateDto.Name) || id < 0)
        {
            return BadRequest("Name is required");
        }
        var customer = await _context.Customers.FindAsync(id);
        if (customer is null)
        {
            return NotFound();
        }

        var mobile = customerUpdateDto?.Mobile?.Trim();
        var nic = customerUpdateDto?.Nic?.Trim().ToLower();
        var email = customerUpdateDto?.Email?.Trim().ToLower();

        if (!string.IsNullOrWhiteSpace(mobile))
        {
            var isMobileExists = await _context.Customers.AnyAsync(c =>
             c.Id != customer.Id &&
             c.Mobile != null &&
             string.Equals(c.Mobile, mobile));

            if (isMobileExists)
            {
                return BadRequest("Mobile is already exists..");
            }
        }
        if (!string.IsNullOrWhiteSpace(nic))
        {
            var isNicExists = await _context.Customers.AnyAsync(c =>
             c.Id != customer.Id &&
             c.Nic != null &&
             string.Equals(c.Nic, nic));

            if (isNicExists)
            {
                return BadRequest("NIC already exists..");
            }
        }
         if (!string.IsNullOrWhiteSpace(email))
        {
            var isEmailExists = await _context.Customers.AnyAsync(c =>
             c.Id != customer.Id &&
             c.Email != null &&
             string.Equals(c.Email, email));

            if (isEmailExists)
            {
                return BadRequest("Email is already Exists..");
            }
        }

        // if (!string.IsNullOrWhiteSpace(customerUpdateDto.Mobile))
        // {
        //     var mobile = customerUpdateDto.Mobile.Trim();
        //     var customerWithExitsMobile = await _context.Customers
        //         .Where(c => c.Mobile != null && string.Equals(c.Mobile, mobile))
        //         .FirstOrDefaultAsync();

        //     if (customerWithExitsMobile is not null)
        //     {
        //         if (customer.Id != customerWithExitsMobile?.Id)
        //         {
        //             return BadRequest("Mobile is already exists..");
        //         }
        //     }


        // }
        // if (!string.IsNullOrWhiteSpace(customerUpdateDto.Nic))
        // {
        //     var nic = customerUpdateDto.Nic.Trim();
        //     var cutomerWithExistsNic = await _context.Customers
        //         .Where(c => c.Nic != null && string.Equals(c.Nic, nic))
        //         .FirstOrDefaultAsync();

        //     if (cutomerWithExistsNic is not null)
        //     {
        //         if (customer.Id != cutomerWithExistsNic?.Id)
        //         {
        //             return BadRequest("NIC already exists..");
        //         }
        //     }
        // }

        // if (!string.IsNullOrWhiteSpace(customerUpdateDto.Email))
        // {
        //     var email = customerUpdateDto.Email.Trim();
        //     var customerWithExitsEmail = await _context.Customers
        //     .Where(c => c.Email != null && string.Equals(c.Email, email))
        //     .FirstOrDefaultAsync();

        //     if (customerWithExitsEmail is not null)
        //     {
        //         if (customer.Id != customerWithExitsEmail?.Id)
        //         {
        //             return BadRequest("Email is already Exists..");
        //         }
        //     }
        // }

        customer.Name = customerUpdateDto.Name;
        customer.Mobile = customerUpdateDto.Mobile;
        customer.Nic = customerUpdateDto.Nic;
        customer.Email = customerUpdateDto.Email;

        await _context.SaveChangesAsync();
        return NoContent();
        // return Ok(customer);
    }

    [Authorize(Policy = "CUSTOMER_DELETE")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        // This is hard delete
        // var customer = await _context.Customers.FindAsync(id);
        // if (customer is null)
        // {
        //     return NotFound("Requested deleted ID is not found...");
        // }
        // _context.Customers.Remove(customer);
        // await _context.SaveChangesAsync();
        // return NoContent();

        // following is soft delete
        var customer = await _context.Customers.FindAsync(id);
        if (customer is null)
        {
            return NotFound("Requested deleted ID is not found...");
        }
        customer.IsActive = false;
        await _context.SaveChangesAsync();
        return NoContent();
    }




}