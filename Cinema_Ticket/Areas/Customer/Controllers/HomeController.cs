
namespace Cinema_Ticket.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRepositroy<Cart> repoCart;
        private readonly IRepositroy<Movie> repoMovie;

        private readonly IRepositroy<Movie> movieRepo;

        public HomeController(IRepositroy<Movie> _movieRepo, UserManager<ApplicationUser> _userManager, IRepositroy<Cart> _repoCart, IRepositroy<Movie> _repoMovie)
        {
            movieRepo = _movieRepo;
            userManager = _userManager;
            repoCart = _repoCart;
            repoMovie = _repoMovie;

        }
        [Authorize]
        public async Task<IActionResult> Index(int movieId, int countItem = 4, int page = 1)
        {
            var movies = await movieRepo.GetAsync(tracked: false);
            ViewBag.countItem = countItem;
            ViewBag.currentPage = page;
            ViewBag.totalPages = (int)Math.Ceiling((double)movies.Count() / countItem);
            movies = movies.Skip((page - 1) * countItem).Take(countItem);
            if (movieId != 0)
            {

                var user = await userManager.GetUserAsync(User);
                var movie = await repoMovie.GetOneAsync(m => m.Id == movieId, tracked: false);
                await repoCart.AddAsync(new Cart
                {
                    MovieId = movieId,
                    UserId = user!.Id,
                    Price = movie!.Price
                });
                await repoCart.CommitAsync();
                TempData["success-notification"] = "Movie added to cart successfully!";
            }
            return View(movies);
        }
    }
}
