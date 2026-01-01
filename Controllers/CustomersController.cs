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

    [HttpPost]
    public async Task<IActionResult> SaveCustomer([FromBody] CustomerAddDto customerAddDto)
    {
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

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerUpdateDto customerUpdateDto)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer is null)
        {
            return NotFound();
        }
        customer.Name = customerUpdateDto.Name;
        customer.Mobile = customerUpdateDto.Mobile;
        customer.Nic = customerUpdateDto.Nic;
        customer.Email = customerUpdateDto.Email;

        await _context.SaveChangesAsync();
        return Ok(customer);
    }
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