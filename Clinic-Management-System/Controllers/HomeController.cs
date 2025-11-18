using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic_Management_System.Controllers
{
    [Authorize]   // ???? ???? ???? ????
    public class HomeController : Controller
    {
        // Dashboard ? ?????? ???
        [Authorize(Roles = "AdminDoctor")]
        public IActionResult Index()
        {
            return View();
        }

        // ?? ????????? ???? ??? /Home/Index ?????? ? ??????? Redirect ??????
        [Authorize(Roles = "Secretary")]
        public IActionResult AccessDeniedRedirect()
        {
            return RedirectToAction("Index", "Receptionists");
        }
    }
}
