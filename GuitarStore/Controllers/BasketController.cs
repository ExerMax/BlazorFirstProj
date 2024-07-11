using ApplicationDbAccess.Models;
using ApplicationDbAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GuitarStore.Controllers
{
    [ApiController]
    [Route("api/basket")]
    public class BasketController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public BasketController(ApplicationDbContext context, ILogger<BasketController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Basket>>> GetAllBaskets()
        {
            return await _context.Baskets.ToListAsync();
        }

        [HttpGet("id")]
        public async Task<ActionResult<IEnumerable<Basket>>> GetBaskets(Guid prodId)
        {
            return await _context.Baskets.Where(basket => basket.ProductId == prodId).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Basket>> PostBasket(Guid productId)
        {
            _logger.LogInformation(productId.ToString());
            _context.Baskets.Add(new Basket { BasketId = Guid.NewGuid(), ProductId = productId });
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Basket), new { });
        }

        [HttpDelete("{prodId}")]
        public async Task<IActionResult> DeleteProduct(Guid prodId)
        {
            var basket = await _context.Baskets.Where(b => b.ProductId == prodId).FirstOrDefaultAsync();
            if (basket == null)
            {
                return NotFound();
            }

            _context.Baskets.Remove(basket);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
