using ApplicationDbAccess;
using ApplicationDbAccess.Models;
using GuitarStore.Components.Pages.AdminPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public UsersController(ApplicationDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpGet("Name={name}&Password={password}")]
        public async Task<User> GetUsers(string name, string password)
        {
            _logger.LogInformation(name);
            _logger.LogInformation(password);
            var user = await _context.Users.Where(u => u.Name == name).FirstOrDefaultAsync();
            user.Password = "";
            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            user.Id = Guid.NewGuid();
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(User), new { id = user.Id }, user);
        }
    }
}
