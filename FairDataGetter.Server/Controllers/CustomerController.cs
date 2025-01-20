using FairDataGetter.Server.Data;
using FairDataGetter.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;

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
        public async Task<IActionResult> PostCustomer([FromForm] CustomerDto customerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (customerDto.Image == null || customerDto.Image.Length == 0)
                return BadRequest("Image is required.");

            // Save the uploaded image
            var uploads = Path.Combine(environment.ContentRootPath, "Uploads");
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            // Generate a unique filename for the uploaded image
            var fileName = $"{Guid.NewGuid()}.png";
            var filePath = Path.Combine(uploads, fileName);

            // Save the uploaded image
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await customerDto.Image.CopyToAsync(stream);
            }

            // Deserialize product group names from DTO
            List<string> productGroupNames = string.IsNullOrEmpty(customerDto.InterestedProductGroupNames)
                ? new List<string>() // Handle null or empty input gracefully
                : System.Text.Json.JsonSerializer.Deserialize<List<string>>(customerDto.InterestedProductGroupNames);

            // Log the product group names
            Debug.WriteLine("API: Product Group Names: " + string.Join(", ", productGroupNames));

            if (!productGroupNames.Any())
            {
                Debug.WriteLine("API: No product groups selected.");
            }

            List<ProductGroup> interestedProductGroups = new List<ProductGroup>();

            if (productGroupNames.Any())
            {
                // Fetch existing product groups from the database
                var existingProductGroups = await context.ProductGroups
                    .Where(pg => productGroupNames
                        .Any(name => pg.Name.ToLower() == name.ToLower()))
                    .ToListAsync();

                Debug.WriteLine("API: Existing Product Groups: " + string.Join(", ", existingProductGroups.Select(pg => pg.Name)));

                // Identify new product groups that need to be created
                var newProductGroups = productGroupNames
                    .Where(name => !existingProductGroups
                        .Any(pg => pg.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    .Select(name => new ProductGroup { Name = name })
                    .ToList();

                Debug.WriteLine("API: New Product Groups to Create: " + string.Join(", ", newProductGroups.Select(pg => pg.Name)));

                // Add new product groups to the database if they don't already exist
                if (newProductGroups.Any())
                {
                    context.ProductGroups.AddRange(newProductGroups);
                    await context.SaveChangesAsync(); // Save new product groups to database
                    existingProductGroups.AddRange(newProductGroups);
                }

                // Use the list of existing product groups to assign to the customer
                interestedProductGroups = existingProductGroups;
            }
            else
            {
                Debug.WriteLine("API: No interested product groups provided.");
            }

            Debug.WriteLine("API: Final Interested Product Groups: " + string.Join(", ", interestedProductGroups.Select(pg => pg.Name)));

            // Check if an address with the same details already exists
            var existingAddress = await context.Addresses
                .FirstOrDefaultAsync(a =>
                    a.Street == customerDto.Address.Street &&
                    a.HouseNumber == customerDto.Address.HouseNumber &&
                    a.City == customerDto.Address.City &&
                    a.PostalCode == customerDto.Address.PostalCode &&
                    a.Country == customerDto.Address.Country);

            int addressId;

            if (existingAddress != null)
            {
                // Reuse existing address
                Debug.WriteLine($"API: Reusing existing address with ID {existingAddress.Id}");
                addressId = existingAddress.Id;
            }
            else
            {
                // Create a new address if it doesn't exist
                var address = new Address
                {
                    Street = customerDto.Address.Street,
                    HouseNumber = customerDto.Address.HouseNumber,
                    City = customerDto.Address.City,
                    PostalCode = customerDto.Address.PostalCode,
                    Country = customerDto.Address.Country
                };

                context.Addresses.Add(address);
                await context.SaveChangesAsync(); // Save new address to get its ID
                addressId = address.Id;

                Debug.WriteLine($"API: Created new address with ID {addressId}");
            }

            // Create and save the Customer with AddressId and Interested Product Groups
            var customer = new Customer
            {
                FirstName = customerDto.FirstName,
                LastName = customerDto.LastName,
                Email = customerDto.Email,
                ImagePath = Path.Combine("Uploads", fileName),
                AddressId = addressId, // Use the existing or newly created AddressId
                InterestedProductGroups = interestedProductGroups, // Many-to-many relationship handled by EF Core
                CompanyId = customerDto.CompanyId
            };

            Debug.WriteLine($"API: Creating customer {customer.FirstName} {customer.LastName} with {interestedProductGroups.Count} product groups.");

            context.Customers.Add(customer);
            await context.SaveChangesAsync(); // Save the customer and establish the many-to-many relationship

            // Ensure the relationship is saved correctly
            Debug.WriteLine($"API: Created customer with ID {customer.Id}");

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }

        // PUT and DELETE methods here
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await context.Customers
                .Include(c => c.InterestedProductGroups) // Ensure related entities are included if necessary
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                return NotFound(); // Return 404 if the customer does not exist
            }

            // Remove the customer from the database
            context.Customers.Remove(customer);
            await context.SaveChangesAsync(); // Persist changes to the database

            return NoContent(); // Return 204 No Content on successful deletion
        }
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
        public string InterestedProductGroupNames { get; set; }

        public int? CompanyId { get; set; }
    }
}
