using Microsoft.AspNetCore.Mvc;

namespace Cinema_Ticket.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
