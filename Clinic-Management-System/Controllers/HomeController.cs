using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic_Management_System.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        [Authorize(Roles = "AdminDoctor")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Secretary")]
        public IActionResult AccessDeniedRedirect()
        {
            return RedirectToAction("Index", "Receptionists");
        }
    }
}
