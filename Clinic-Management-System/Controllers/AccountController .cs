using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Clinic_Management_System.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(SignInManager<IdentityUser> signInManager,
                                 UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "يجب إدخال البريد وكلمة المرور.";
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ViewBag.Error = "المستخدم غير موجود.";
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

            if (!result.Succeeded)
            {
                ViewBag.Error = "خطأ في تسجيل الدخول.";
                return View();
            }

            if (await _userManager.IsInRoleAsync(user, "AdminDoctor"))
                return RedirectToAction("Index", "Home");

            if (await _userManager.IsInRoleAsync(user, "Secretary"))
                return RedirectToAction("GetAll", "Patient");

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
