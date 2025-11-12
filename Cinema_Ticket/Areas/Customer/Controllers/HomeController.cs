using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cinema_Ticket.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IRepositroy<Movie> movieRepo;

        public HomeController(IRepositroy<Movie> _movieRepo)
        {
            movieRepo = _movieRepo;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await movieRepo.GetAsync(tracked:false);
            return View(movies);
        }
    }
}
