using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cinema_Ticket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> ShowAll()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }
    }
}
