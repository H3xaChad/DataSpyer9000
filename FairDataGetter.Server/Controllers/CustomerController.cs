// Controllers/CustomersController.cs
using FairDataGetter.Server.Data;
using FairDataGetter.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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

        // POST: api/Customers
        [HttpPost]
        public async Task<IActionResult> PostCustomer([FromForm] CustomerDto customerDto) {
            if (customerDto.Image == null || customerDto.Image.Length == 0)
                return BadRequest("Image is required.");

            // Save image to disk
            var uploads = Path.Combine(_environment.ContentRootPath, "uploads");
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(customerDto.Image.FileName)}";
            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create)) {
                await customerDto.Image.CopyToAsync(stream);
            }

            // Handle Interested Product Groups
            var productGroupNames = customerDto.InterestedProductGroupNames
                .Select(name => name.Trim())
                .Where(name => !string.IsNullOrEmpty(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var existingProductGroups = await _context.ProductGroups
                .Where(pg => productGroupNames.Contains(pg.Name))
                .ToListAsync();

            var newProductGroups = productGroupNames
                .Where(name => !existingProductGroups.Any(pg => pg.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                .Select(name => new ProductGroup { Name = name })
                .ToList();

            if (newProductGroups.Any()) {
                _context.ProductGroups.AddRange(newProductGroups);
                await _context.SaveChangesAsync();

                // Update existingProductGroups with newly added ones
                existingProductGroups.AddRange(newProductGroups);
            }

            // Associate ProductGroups with Customer
            var interestedProductGroups = existingProductGroups
                .Where(pg => productGroupNames.Contains(pg.Name, StringComparer.OrdinalIgnoreCase))
                .ToList();

            var customer = new Customer {
                FirstName = customerDto.FirstName,
                LastName = customerDto.LastName,
                Email = customerDto.Email,
                ImagePath = Path.Combine("uploads", fileName),
                InterestedProductGroups = interestedProductGroups
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id) {
            var customer = await _context.Customers
                .Include(c => c.InterestedProductGroups)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                return NotFound();

            return customer;
        }
    }

    // DTO for binding form data
    public class CustomerDto {
        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required]
        public required string Email { get; set; }

        [Required]
        public required IFormFile Image { get; set; }

        [Required]
        public required List<string> InterestedProductGroupNames { get; set; }
    }
}
