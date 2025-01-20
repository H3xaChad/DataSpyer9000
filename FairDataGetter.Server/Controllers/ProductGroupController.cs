using FairDataGetter.Server.Data;
using FairDataGetter.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FairDataGetter.Server.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ProductGroupsController(AppDbContext context) : ControllerBase {

        // GET: api/ProductGroups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductGroup>>> GetProductGroups() {
            return await context.ProductGroups.ToListAsync();
        }

        // GET: api/ProductGroups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductGroup>> GetProductGroup(int id) {
            var productGroup = await context.ProductGroups.FindAsync(id);

            if (productGroup == null)
                return NotFound();

            return productGroup;
        }

        // POST: api/ProductGroups
        [HttpPost]
        public async Task<ActionResult<IEnumerable<ProductGroup>>> PostProductGroup([FromBody] ProductGroup productGroup)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if the provided name contains multiple entries
            var names = productGroup.Name.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var createdGroups = new List<ProductGroup>();

            foreach (var name in names)
            {
                // Check if a ProductGroup with the same name already exists
                bool exists = await context.ProductGroups
                    .AnyAsync(pg => pg.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

                if (!exists)
                {
                    // Create a new ProductGroup
                    var newProductGroup = new ProductGroup { Name = name };
                    context.ProductGroups.Add(newProductGroup);
                    createdGroups.Add(newProductGroup);
                }
            }

            // Save changes only once for performance
            if (createdGroups.Any())
                await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductGroups), createdGroups);
        }


        // PUT and DELETE methods here
    }
}
