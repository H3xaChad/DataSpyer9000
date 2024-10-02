// Controllers/CustomersController.cs
using FairDataGetter.Server.Data;
using FairDataGetter.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace FairDataGetter.Server.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CustomersController(AppDbContext context, IWebHostEnvironment environment) {
            _context = context;
            _environment = environment;
        }

        [HttpPost]
        public async Task<IActionResult> PostCustomer([FromForm] CustomerDto customerDto) {
            if (customerDto.Image == null || customerDto.Image.Length == 0)
                return BadRequest("Image is required.");

            var uploads = Path.Combine(_environment.ContentRootPath, "uploads");
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var fileName = $"{Guid.NewGuid()}_{customerDto.Image.FileName}";
            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create)) {
                await customerDto.Image.CopyToAsync(stream);
            }

            var customer = new Customer {
                FirstName = customerDto.FirstName,
                LastName = customerDto.LastName,
                Email = customerDto.Email,
                ImagePath = Path.Combine("uploads", fileName)
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id) {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            return customer;
        }
    }

    // DTO for binding form data
    public class CustomerDto {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required IFormFile Image { get; set; }
    }
}
