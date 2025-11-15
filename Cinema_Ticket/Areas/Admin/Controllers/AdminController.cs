using Microsoft.AspNetCore.Mvc;

namespace Cinema_Ticket.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{StaticRole.SUPER_ADMIN} , {StaticRole.ADMIN}")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
