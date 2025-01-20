using FairDataGetter.Server.Data;
using FairDataGetter.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FairDataGetter.Server.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController(AppDbContext context) : ControllerBase {

        // GET: api/Addresses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddresses() {
            return await context.Addresses.ToListAsync();
        }

        // GET: api/Address/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Address>> GetAddress(int id) {
            var address = await context.Addresses.FindAsync(id);

            if (address == null)
                return NotFound();

            return address;
        }

        // POST: api/Address
        [HttpPost]
        public async Task<ActionResult<Address>> PostAddress([FromBody] Address address)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check for duplicate addresses based on key fields
            var existingAddress = await context.Addresses
                .FirstOrDefaultAsync(a =>
                    a.Street.ToLower() == address.Street.ToLower() &&
                    a.HouseNumber.ToLower() == address.HouseNumber.ToLower() &&
                    a.City.ToLower() == address.City.ToLower() &&
                    a.PostalCode.ToLower() == address.PostalCode.ToLower() &&
                    a.Country.ToLower() == address.Country.ToLower());

            if (existingAddress != null)
            {
                // Return the ID of the existing address
                return Conflict(new { message = "An Address with the same details already exists.", existingAddressId = existingAddress.Id });
            }

            // Add the new address if no match is found
            context.Addresses.Add(address);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAddress), new { id = address.Id }, address);
        }

        // PUT: api/Address/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAddress(int id, [FromBody] Address address) {
            if (id != address.Id)
                return BadRequest("ID mismatch.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            context.Entry(address).State = EntityState.Modified;

            try {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!AddressExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Address/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id) {
            var address = await context.Addresses.FindAsync(id);
            if (address == null)
                return NotFound();

            context.Addresses.Remove(address);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool AddressExists(int id) {
            return context.Addresses.Any(e => e.Id == id);
        }
    }
}
