namespace Cinema_Ticket.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class RegisterController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailSender emailSender;

        public RegisterController(UserManager<ApplicationUser> _userManager , IEmailSender _emailSender)
        {
            userManager = _userManager;
            emailSender = _emailSender;
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            registerVM.FullName = AppConfiguration.TrimExtraSpace(registerVM.FullName);
            var user = new ApplicationUser
            {
                Email = registerVM.Email,
                FirstName = registerVM.FullName.Split(' ')[0],
                LastName = registerVM.FullName.Split(' ')[1],
                UserName = $"{registerVM.FullName.Split(' ')[0].ToLower()}{registerVM.FullName.Split(' ')[1].ToLower()}"
            };
            var result = await userManager.CreateAsync(user, registerVM.Password);
            if(!result.Succeeded)
            {
                StringBuilder builder = new();
                foreach (var item in result.Errors)
                    builder.AppendLine(item.Description);
                TempData["error-notification"] = builder.ToString();
                return View(registerVM);
            }
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(EmailConfirmation) , "Register", new {area = "Identity" , token , userId = user.Id },Request.Scheme);
            await emailSender.SendEmailAsync(user.Email, "Cinema-Confirm Your Email", $"<h1> To Confirm Your Email Click <a href='{link}'> Here </a></h1>");
            TempData["success-notification"] = "Registration Successful. Please check your email to confirm your account.";
            return View(registerVM);
        }
        public async Task<IActionResult> EmailConfirmation(string userId, string token)
        {
            var user = await userManager.FindByIdAsync(userId);
            var result = await userManager.ConfirmEmailAsync(user!, token);
            if (!result.Succeeded)
            {
                StringBuilder errors = new();
                foreach (var item in result.Errors)
                    errors.AppendLine(item.Description);
                TempData["error-notification"] = errors.ToString();
                return RedirectToAction(nameof(Register));
            }
            TempData["success-notification"] = "Email confirmed successfully. You can now log in.";
            return View(nameof(LogIn));
        }
        public IActionResult LogIn()
        {
            return View();
        }
    }
}
