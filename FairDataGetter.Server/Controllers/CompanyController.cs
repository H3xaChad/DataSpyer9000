using FairDataGetter.Server.Data;
using FairDataGetter.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FairDataGetter.Server.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController(AppDbContext context) : ControllerBase {

        // GET: api/Company
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetAllCompanies() {
            var companies = await context.Companies
                .Include(c => c.Address)
                .ToListAsync();
            return Ok(companies);
        }

        // GET: api/Company/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Company>> GetCompanyById(int id) {
            var company = await context.Companies
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (company == null)
                return NotFound();

            return Ok(company);
        }

        // POST: api/Company
        [HttpPost]
        public async Task<ActionResult<Company>> CreateCompany([FromBody] CompanyPostDto companyDto) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingCompany = await context.Companies
                .FirstOrDefaultAsync(c => EF.Functions.Like(c.Name, companyDto.Name));

            if (existingCompany != null)
                return Ok(new { message = "Company already exists.", id = existingCompany.Id });

            var address = await context.Addresses.FindAsync(companyDto.AddressId);
            if (address == null)
                return BadRequest(new { message = "Address does not exist." });

            var company = companyDto.ToCompany(address);
            context.Companies.Add(company);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCompanyById), new { id = company.Id }, company);
        }

        // DELETE: api/Company/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCompany(int id) {
            var company = await context.Companies.FindAsync(id);
            if (company == null)
                return NotFound();

            context.Companies.Remove(company);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}