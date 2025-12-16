using System.Threading.Tasks;

namespace Cinema_Ticket.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class RegisterController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailSender emailSender;
        private readonly SignInManager<ApplicationUser> signManager;
        private readonly IRepositroy<ApplicationUserOTP> repoOTP;

        public RegisterController(UserManager<ApplicationUser> _userManager, IEmailSender _emailSender , SignInManager<ApplicationUser> _signManager , IRepositroy<ApplicationUserOTP> _repoOTP)
        {
            userManager = _userManager;
            emailSender = _emailSender;
            signManager = _signManager;
            repoOTP = _repoOTP;
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
            if (!result.Succeeded)
            {
                StringBuilder builder = new();
                foreach (var item in result.Errors)
                    builder.AppendLine(item.Description);
                TempData["error-notification"] = builder.ToString();
                return View(registerVM);
            }
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(EmailConfirmation), "Register", new { area = "Identity", token, userId = user.Id }, Request.Scheme);
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
        [HttpPost]
        public async Task<IActionResult> LogIn(LoginVM loginVM)
        {
            var user =await userManager.FindByEmailAsync(loginVM.Email);
            ViewBag.Email = loginVM.Email;
            ViewBag.Password = loginVM.Password;
            if(user == null)
            {
                TempData["error-notification"] = "Invalid Email or Password.";
                return View();
            }
            if (!user.EmailConfirmed)
            {
                TempData["error-notification"] = "Email Must Confirmed Befor Login.";
                return View();
            }
            
            var checkPass = await signManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, true);
            if(!checkPass.Succeeded)
            {
                if (checkPass.IsLockedOut)
                    TempData["error-notification"] = "Your account is locked. Please try again later.";
                else
                    TempData["error-notification"] = "Invalid Email or Password.";
                return View();
            }
            ViewBag.RememberMe = loginVM.RememberMe;
            TempData["success-notification"] = "Logged in successfully.";
            return RedirectToAction("Index","Home" , new {area = "Customer" });
        }
        public IActionResult Logout()
        {
            signManager.SignOutAsync();
            TempData["success-notification"] = "Logged out successfully.";
            return RedirectToAction(nameof(LogIn));
        }
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if(user is null)
            {
                TempData["error-notification"] = "No user found with this email.";
                return View();
            }
            var otpsCount = await repoOTP.GetAsync(o => o.UserId == user.Id && o.CreatedAt.Day == DateTime.Now.Day);
            if(otpsCount.Count() >= 3)
            {
                TempData["error-notification"] = "You have reached the maximum number of password reset requests. Please try again after 24 hours.";
                return View();
            }
            var otpCode = new Random().Next(100000, 999999);
            var userOTP = new ApplicationUserOTP
            {
                UserId = user.Id,
                OTP = otpCode.ToString(),
            };
            await repoOTP.AddAsync(userOTP);
            await repoOTP.CommitAsync();
            await emailSender.SendEmailAsync(user.Email!, "Cinema-Password Reset OTP", $"<h1> Your OTP for password reset is: {otpCode} </h1>");
            return RedirectToAction(nameof(OTPVerification) , new { userId = user.Id});
        }
        public async Task<IActionResult> OTPVerification(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> OTPVerification(string userId, string otp)
        {
            var userOtp = await repoOTP.GetOneAsync(o => o.UserId == userId && o.OTP == otp);
            if(userOtp is null)
            {
                TempData["error-notification"] = "Invalid OTP. Please try again.";
                return View();
            }
            TempData["success-notification"] = "OTP verified successfully. You can now reset your password.";
            return RedirectToAction(nameof(ResetPassword) , new { userId });
        }
        public async Task<IActionResult> ResetPassword(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string userId , ResetPasswordVM reset)
        {
            var user = await userManager.FindByIdAsync(userId);
            var token = await userManager.GeneratePasswordResetTokenAsync(user!);
            var result = await userManager.ResetPasswordAsync(user!, token, reset.Password);
            if(!result.Succeeded)
            {
                StringBuilder errors = new();
                foreach (var item in result.Errors)
                    errors.AppendLine(item.Description);
                TempData["error-notification"] = errors.ToString();
                return View();
            }
            TempData["success-notification"] = "Password reset successfully. You can now log in with your new password.";
            return View("BackToLogin");
        }
    }
}
