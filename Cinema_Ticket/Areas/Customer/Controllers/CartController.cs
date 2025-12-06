

using Stripe.Checkout;

namespace Cinema_Ticket.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRepositroy<Cart> repoCart;


        public CartController(UserManager<ApplicationUser> _userManager, IRepositroy<Cart> _repoCart, IRepositroy<Movie> _repoMovie)
        {
            userManager = _userManager;
            repoCart = _repoCart;
        }

        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);
            var cart = await repoCart.GetAsync(c => c.UserId == user!.Id, includes: [c => c.Movie]);
            return View(cart);
        }
        public async Task<IActionResult> Remove(int movieId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null)
            {
                TempData["error-notification"] = "User Not Found.";
                return RedirectToAction("Index");
            }
            var removeMovie = await repoCart.GetOneAsync(rm => rm.MovieId == movieId && rm.UserId == user.Id);
            if (removeMovie is null)
            {
                TempData["error-notification"] = "Movie Not Found.";
                return RedirectToAction("Index");
            }

            repoCart.Delete(removeMovie);
            await repoCart.CommitAsync();
            TempData["success-notification"] = "Movie Is Deleted Successfully.";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Pay()
        {
            var user = await userManager.GetUserAsync(User);
            var cart = await repoCart.GetAsync(u => u.UserId == user!.Id, includes: [m => m.Movie]);
            var option = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/customer/checkout/success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/customer/checkout/cancel"
            };
            foreach (var item in cart)
            {
                option.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Movie.Name,
                            Description = item.Movie.Description,
                        },
                        UnitAmount = (long)item.Price * 100,
                    },
                    Quantity = item.Quantity,
                });
            }
            var service = new SessionService();
            var session = await service.CreateAsync(option);
            return Redirect(session.Url);
        }
    }
}
