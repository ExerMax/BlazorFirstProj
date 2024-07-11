using ApplicationDbAccess.Models;
using ApplicationDbAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Controllers
{
    [ApiController]
    [Route("api/Orders")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public OrdersController(ApplicationDbContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        [HttpGet("id")]
        public async Task<ActionResult<IEnumerable<Order>>> GetBaskets(Guid userId)
        {
            return await _context.Orders.Where(basket => basket.UserId == userId).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Basket>> PostBasket(Guid userId, Guid productId)
        {
            var dateTime = DateTime.Now;
            _context.Orders.Add(new Order { OrderId = Guid.NewGuid(), UserId = userId, Status = "Новый", DateTime = dateTime.ToString()});
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Basket), new { UserId = userId, ProductId = productId });
        }
    }
}
