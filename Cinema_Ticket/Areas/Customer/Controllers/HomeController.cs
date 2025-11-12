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

        public async Task<IActionResult> Index(int countItem = 4,int page = 1)
        {
            var movies = await movieRepo.GetAsync(tracked:false);
            ViewBag.countItem = countItem;
            ViewBag.currentPage = page;
            ViewBag.totalPages = (int)Math.Ceiling((double)movies.Count() / countItem);
            movies = movies.Skip((page - 1) * countItem).Take(countItem);
            return View(movies);
        }
    }
}
