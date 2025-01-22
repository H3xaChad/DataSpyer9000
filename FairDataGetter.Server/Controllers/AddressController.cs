using FairDataGetter.Server.Data;
using FairDataGetter.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FairDataGetter.Server.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController(AppDbContext context) : ControllerBase {

        // GET: api/Address
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Address>>> GetAllAddresses() {
            var addresses = await context.Addresses.ToListAsync();
            return Ok(addresses);
        }

        // GET: api/Address/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Address>> GetAddressById(int id) {
            var address = await context.Addresses.FindAsync(id);
            if (address == null)
                return NotFound();

            return Ok(address);
        }

        // POST: api/Address
        [HttpPost]
        public async Task<ActionResult<Address>> CreateAddress([FromBody] AddressPostDto addressDto) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            // Check if the address already exists, so we don't create duplicates
            var existingAddress = await context.Addresses
                .FirstOrDefaultAsync(a =>
                    EF.Functions.Like(a.Street, addressDto.Street) &&
                    EF.Functions.Like(a.HouseNumber, addressDto.HouseNumber) &&
                    EF.Functions.Like(a.City, addressDto.City) &&
                    EF.Functions.Like(a.PostalCode, addressDto.PostalCode) &&
                    EF.Functions.Like(a.Country, addressDto.Country));

            if (existingAddress != null)
                return Ok(new { message = "Address already exists.", id = existingAddress.Id });

            var address = addressDto.ToAddress();
            context.Addresses.Add(address);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAddressById), new { id = address.Id }, address);
        }

        // DELETE: api/Address/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAddress(int id) {
            var address = await context.Addresses.FindAsync(id);
            if (address == null)
                return NotFound();

            context.Addresses.Remove(address);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}