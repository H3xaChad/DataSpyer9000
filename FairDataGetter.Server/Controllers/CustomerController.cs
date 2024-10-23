using FairDataGetter.Server.Data;
using FairDataGetter.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FairDataGetter.Server.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController(AppDbContext context, IWebHostEnvironment environment) : ControllerBase {

        //public string GenerateFileName(string firstName, string lastName, string originalFileName) {
        //    // Sanitize the names by removing special characters and making it lowercase
        //    string sanitizedFirstName = RemoveSpecialCharacters(firstName).ToLower();
        //    string sanitizedLastName = RemoveSpecialCharacters(lastName).ToLower();

        //    // Generate a short version of a GUID (first 8 characters)
        //    string shortGuid = Guid.NewGuid().ToString().Split('-')[0];

        //    // Combine name parts with the short GUID
        //    string fileName = $"{sanitizedFirstName}_{sanitizedLastName}_{shortGuid}";

        //    // Ensure file extension from the original file name is preserved
        //    string extension = Path.GetExtension(originalFileName);

        //    // Optionally pad the file name to a specific length (e.g., 40 characters)
        //    fileName = fileName.Length > 40 ? fileName[..40] : fileName.PadRight(40, '_');

        //    // Return the final file name with the extension
        //    return $"{fileName}{extension}";
        //}

        //private string RemoveSpecialCharacters(string str) {
        //    return new string(str.Where(c => char.IsLetterOrDigit(c)).ToArray());
        //}

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers() {
            return await context.Customers
                .Include(c => c.InterestedProductGroups)
                .ToListAsync();
        }

        // GET: api/Customers{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id) {
            var customer = await context.Customers
                .Include(c => c.InterestedProductGroups)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                return NotFound();

            return customer;
        }

        // POST: api/Customers
        [HttpPost]
        public async Task<IActionResult> PostCustomer([FromForm] CustomerDto customerDto) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (customerDto.Image == null || customerDto.Image.Length == 0)
                return BadRequest("Image is required.");

            var uploads = Path.Combine(environment.ContentRootPath, "Uploads");
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var fileName = customerDto.Image.FileName;//GenerateFileName(customerDto.FirstName, customerDto.LastName, customerDto.Image.FileName);
            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create)) {
                await customerDto.Image.CopyToAsync(stream);
            }

            var productGroupNames = customerDto.InterestedProductGroupNames
                .Select(name => name.Trim())
                .Where(name => !string.IsNullOrEmpty(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var existingProductGroups = await context.ProductGroups
                .Where(pg => productGroupNames.Contains(pg.Name))
                .ToListAsync();

            var newProductGroups = productGroupNames
                .Where(name => !existingProductGroups.Any(pg => pg.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                .Select(name => new ProductGroup { Name = name })
                .ToList();

            if (newProductGroups.Count != 0) {
                context.ProductGroups.AddRange(newProductGroups);
                await context.SaveChangesAsync();
                existingProductGroups.AddRange(newProductGroups);
            }

            var interestedProductGroups = existingProductGroups
                .Where(pg => productGroupNames.Contains(pg.Name, StringComparer.OrdinalIgnoreCase))
                .ToList();

            var customer = new Customer {
                FirstName = customerDto.FirstName,
                LastName = customerDto.LastName,
                Email = customerDto.Email,
                ImagePath = Path.Combine("Uploads", fileName),
                Address = customerDto.Address,
                InterestedProductGroups = interestedProductGroups
            };

            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }

        // PUT and DELETE methods here
    }

    public class CustomerDto {
        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required IFormFile Image { get; set; }

        [Required]
        public required Address Address { get; set; }

        [Required]
        public required List<string> InterestedProductGroupNames { get; set; }
    }

}
