// Controllers/CustomersController.cs
using FairDataGetter.Server.Data;
using FairDataGetter.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FairDataGetter.Server.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context) {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers() {
            return await _context.Customers.ToListAsync();
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id) {
            var Customer = await _context.Customers.FindAsync(id);

            if (Customer == null) {
                return NotFound();
            }

            return Customer;
        }

        // POST: api/Customers
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer Customer) {
            _context.Customers.Add(Customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = Customer.Id }, Customer);
        }

        // PUT: api/Customers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer Customer) {
            if (id != Customer.Id) {
                return BadRequest();
            }

            _context.Entry(Customer).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!CustomerExists(id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id) {
            var Customer = await _context.Customers.FindAsync(id);
            if (Customer == null) {
                return NotFound();
            }

            _context.Customers.Remove(Customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(int id) {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
