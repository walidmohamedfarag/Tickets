
using System.Threading.Tasks;

namespace Cinema_Ticket.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRepositroy<Cart> repoCart;
        private readonly IRepositroy<Movie> repoMovie;


        public CartController(UserManager<ApplicationUser> _userManager , IRepositroy<Cart> _repoCart , IRepositroy<Movie> _repoMovie)
        {
            userManager = _userManager;
            repoCart = _repoCart;
            repoMovie = _repoMovie;
        }

        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);
            var cart = await repoCart.GetAsync(c => c.UserId == user!.Id, includes: [c => c.Movie]);
            return View(cart);
        }
    }
}
