using ApplicationDbAccess;
using ApplicationDbAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public ProductsController(ApplicationDbContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(Guid id)
        {
            var product = await _context.Products.FindAsync(id);

            return product != null ? product : NotFound();
        }

        [HttpGet("Category={category}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct(string category)
        {
            return await _context.Products.Where(c => c.Category == category).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            product.Id = Guid.NewGuid();
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Product), new { id = product.Id }, product);
        }

        [HttpPut]
        public async Task<IActionResult> PutProduct(Product newProduct)
        {
            //var product = await _context.Products.FindAsync(newProduct.Id);

            if (await _context.Products.FindAsync(newProduct.Id) == null)
            {
                _logger.LogInformation("Not Found");
                return NotFound();
            }

            _logger.LogInformation("Found");

            _context.Entry(newProduct).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Saved");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(newProduct.Id))
                {
                    _logger.LogInformation("Not Found2");
                    return NotFound();
                }
                else
                {
                    _logger.LogInformation("Exception");
                    throw;
                }
            }

            _logger.LogInformation("OK");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(Guid id)
        {
            return _context.Products.Any(p => p.Id == id);
        }
    }
}
