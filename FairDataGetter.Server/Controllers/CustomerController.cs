using FairDataGetter.Server.Data;
using FairDataGetter.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FairDataGetter.Server.Controllers {
    
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController(AppDbContext context) : ControllerBase {
        
        private const string CustomerImageFolder = "Uploads";

        // GET: api/Customer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerGetDto>>> GetAllCustomers() {
            var customers = await context.Customers
                .Include(c => c.Address)
                .Include(c => c.Company)
                .ToListAsync();
            
            return Ok(customers.Select(CustomerGetDto.FromCustomer).ToList());
        }

        // GET: api/Customer/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CustomerGetDto>> GetCustomerById(int id) {
            var customer = await context.Customers
                .Include(c => c.Address)
                .Include(c => c.Company)
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (customer == null)
                return NotFound();

            return Ok(CustomerGetDto.FromCustomer(customer));
        }
        
        // GET: api/Customer/{id}/image
        [HttpGet("{id:int}/image")]
        public async Task<IActionResult> GetCustomerImage(int id) {
            var customer = await context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();
            
            var imagePath = Path.Combine(CustomerImageFolder, customer.ImageFileName);
            if (!System.IO.File.Exists(imagePath))
                return NotFound();
            
            // Dynamically determine the MIME type
            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(imagePath, out var contentType)) {
                contentType = "application/octet-stream"; // Fallback MIME type
            }
            
            var image = System.IO.File.OpenRead(imagePath);
            return File(image, contentType);
        }

        // POST: api/Customer
        [HttpPost]
        public async Task<ActionResult<CustomerGetDto>> CreateCustomer([FromForm] CustomerPostDto customerDto) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif" };
            var fileExtension = Path.GetExtension(customerDto.Image.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension)) {
                return BadRequest(new { message = "Invalid image format. Allowed formats are: .png, .jpg, .jpeg, .gif." });
            }
            
            var address = await context.Addresses.FindAsync(customerDto.AddressId);
            if (address == null) 
                return BadRequest(new { message = "Address does not exist." });
            
            var imageFileName = $"{Guid.NewGuid()}{fileExtension}";
            var imagePath = Path.Combine(CustomerImageFolder, imageFileName);
            
            await using (var stream = new FileStream(imagePath, FileMode.Create)) {
                await customerDto.Image.CopyToAsync(stream);
            }
            
            var customer = customerDto.ToCustomer(imageFileName, address);
            context.Customers.Add(customer);
            await context.SaveChangesAsync();
            var customerDtoResponse = CustomerGetDto.FromCustomer(customer);
            return CreatedAtAction(nameof(GetCustomerById), new { id = customerDtoResponse.Id }, customerDtoResponse);
        }

        // DELETE: api/Customer/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCustomer(int id) {
            var customer = await context.Customers.FindAsync(id);
            if (customer == null) 
                return NotFound();
            
            var imagePath = Path.Combine(CustomerImageFolder, customer.ImageFileName);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
            
            context.Customers.Remove(customer);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}