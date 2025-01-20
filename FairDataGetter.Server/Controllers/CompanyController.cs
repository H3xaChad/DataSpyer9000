using FairDataGetter.Server.Data;
using FairDataGetter.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FairDataGetter.Server.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController(AppDbContext context) : ControllerBase {

        // GET: api/Companies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            return await context.Companies.ToListAsync();
        }

        // GET: api/Companies/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            var company = await context.Companies.FirstOrDefaultAsync(c => c.Id == id);

            if (company == null)
                return NotFound();

            return company;
        }

        // POST: api/Companies
        [HttpPost]
        public async Task<ActionResult<Company>> PostCompany([FromBody] CompanyDto companyDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if a company with the same name and address already exists
            var existingCompany = await context.Companies
                .FirstOrDefaultAsync(c =>
                    c.Name.ToLower() == companyDto.Name.ToLower() &&
                    context.Addresses.Any(a =>
                        a.Id == c.AddressId &&
                        a.Street == companyDto.Address.Street &&
                        a.HouseNumber == companyDto.Address.HouseNumber &&
                        a.City == companyDto.Address.City &&
                        a.PostalCode == companyDto.Address.PostalCode &&
                        a.Country == companyDto.Address.Country));

            if (existingCompany != null)
            {
                // Return the ID of the existing company
                return Ok(new { Id = existingCompany.Id });
            }

            // Ensure the AddressId exists, check if an address already exists with the same details
            var existingAddress = await context.Addresses
                .FirstOrDefaultAsync(a =>
                    a.Street == companyDto.Address.Street &&
                    a.HouseNumber == companyDto.Address.HouseNumber &&
                    a.City == companyDto.Address.City &&
                    a.PostalCode == companyDto.Address.PostalCode &&
                    a.Country == companyDto.Address.Country);

            int addressId;

            if (existingAddress != null)
            {
                // Reuse existing address
                addressId = existingAddress.Id;
            }
            else
            {
                // If address does not exist, create a new one
                var address = new Address
                {
                    Street = companyDto.Address.Street,
                    HouseNumber = companyDto.Address.HouseNumber,
                    City = companyDto.Address.City,
                    PostalCode = companyDto.Address.PostalCode,
                    Country = companyDto.Address.Country
                };

                context.Addresses.Add(address);
                await context.SaveChangesAsync(); // Save to get its ID
                addressId = address.Id;
            }

            // Create and save the Company with AddressId
            var company = new Company
            {
                Name = companyDto.Name,
                AddressId = addressId
            };

            // Check if a company with the same name already exists
            /*
            bool exists = await context.Companies
                .AnyAsync(c => c.Name.ToLower() == company.Name.ToLower());

            if (exists)
                return Conflict("A company with the same name already exists.");
            */

            // Add the company to the database
            context.Companies.Add(company);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, new { Id = company.Id });

        }


        // PUT

        // DELETE: api/Companies/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var company = await context.Companies.FindAsync(id);

            if (company == null)
            {
                return NotFound(); // Return 404 if the company does not exist
            }

            context.Companies.Remove(company); // Remove the company from the database
            await context.SaveChangesAsync(); // Persist changes to the database

            return NoContent(); // Return 204 No Content on successful deletion
        }

    }


    public class CompanyDto
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required Address Address { get; set; }

    }
}
