
using System.Threading.Tasks;

namespace Cinema_Ticket.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRepositroy<Cart> repoCart;
        private readonly IRepositroy<Movie> repoMovie;
        private readonly IRepositroy<MovieActor> repoMovieActor;

        private readonly IRepositroy<Movie> movieRepo;

        public HomeController(IRepositroy<Movie> _movieRepo, UserManager<ApplicationUser> _userManager, IRepositroy<Cart> _repoCart, IRepositroy<Movie> _repoMovie, IRepositroy<MovieActor> _repoMovieActor)
        {
            movieRepo = _movieRepo;
            userManager = _userManager;
            repoCart = _repoCart;
            repoMovie = _repoMovie;
            repoMovieActor = _repoMovieActor;
        }
        public async Task<IActionResult> Index(int movieId, int countItem = 4, int page = 1)
        {
            var movies = await movieRepo.GetAsync(tracked: false);
            ViewBag.countItem = countItem;
            ViewBag.currentPage = page;
            ViewBag.totalPages = (int)Math.Ceiling((double)movies.Count() / countItem);
            movies = movies.Skip((page - 1) * countItem).Take(countItem);
            var userLogin = await userManager.GetUserAsync(User);
            if (userLogin is null)
            {
                TempData["error-notification"] = "To Use Cart Must Be Logining";
                return View(movies);
            }
            if (movieId != 0)
            {
                var existsInCart = await repoCart.GetOneAsync(c => c.MovieId == movieId && c.UserId == userLogin.Id, tracked: false);
                if (existsInCart is not null)
                {
                    TempData["error-notification"] = "Movie Already Exists In Cart";
                    return RedirectToAction("Index" , "Cart");
                }
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
        public async Task<IActionResult> Details(int movieId)
        {
            var movie = await repoMovie.GetOneAsync(m => m.Id == movieId, tracked: false, includes: [c => c.Cinema!]);
            if (movie is null)
            {
                TempData["error-notification"] = "Movie Not Found";
                return View(nameof(Index));
            }
            var actorsMovie = await repoMovieActor.GetAsync(ma => ma.MovieId == movie.Id, tracked: false, includes: [a => a.Actor]);
            var movieWithSameType = await repoMovie.GetAsync(m=>m.Type == movie.Type && m.Id != movieId, tracked: false);
            return View(new MovieDetailVM { Actors = actorsMovie , Movie = movie , MoviesWithSameType = movieWithSameType});
        }
    }
}
