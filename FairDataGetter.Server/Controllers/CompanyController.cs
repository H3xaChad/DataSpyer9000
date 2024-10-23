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
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies() {
            return await context.Companies
                .Include(c => c.Address)
                .ToListAsync();
        }

        // GET: api/Companies/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(int id) {
            var company = await context.Companies
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null)
                return NotFound();

            return company;
        }

        // POST: api/Companies
        [HttpPost]
        public async Task<ActionResult<Company>> PostCompany([FromBody] Company company) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool exists = await context.Companies
                .AnyAsync(c => c.Name.Equals(company.Name, StringComparison.OrdinalIgnoreCase));

            if (exists)
                return Conflict("A company with the same name already exists.");

            context.Companies.Add(company);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, company);
        }

        // PUT and DELETE methods here
    }
}
