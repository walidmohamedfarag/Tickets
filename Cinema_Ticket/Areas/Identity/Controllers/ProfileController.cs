using Microsoft.AspNetCore.Mvc;

namespace Cinema_Ticket.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class ProfileController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
