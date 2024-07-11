using ApplicationDbAccess.Models;
using ApplicationDbAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Controllers
{
    [ApiController]
    [Route("api/OrderItems")]
    public class OrderItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public OrderItemsController(ApplicationDbContext context, ILogger<OrderItemsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetAllOrderItems()
        {
            return await _context.OrderItems.ToListAsync();
        }

        [HttpGet("id")]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetBaskets(Guid orderId)
        {
            return await _context.OrderItems.Where(oi => oi.OrderId == orderId).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<OrderItem>> PostBasket(Guid orderId, Guid productId)
        {
            _context.OrderItems.Add(new OrderItem { OrderId = orderId, ProductId = productId });
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(OrderItem), new { OrderId = orderId, ProductId = productId });
        }
    }
}
