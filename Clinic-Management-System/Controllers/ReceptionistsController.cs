using Clinic_Management_System.Models;
using Clinic_Management_System.Services.Receptionists;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]
    public class ReceptionistsController : Controller
    {
        private readonly IReceptionistService _receptionistService;

        public ReceptionistsController(IReceptionistService receptionistService)
        {
            _receptionistService = receptionistService;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _receptionistService.GetReceptionistsAsync();
            return View(list);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var receptionist = await _receptionistService.GetReceptionistByIdAsync(id);
            if (receptionist == null)
                return NotFound();

            return View(receptionist);
        }

        public IActionResult Create()
        {
            var model = new Receptionist
            {
                HiringDate = System.DateTime.Now,
                IsActive = true
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,Salary,Shift,Phone,Role")] Receptionist receptionist)
        {
            if (ModelState.IsValid)
            {
                var result = await _receptionistService.CreateReceptionistAsync(receptionist);
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(receptionist);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var receptionist = await _receptionistService.FindReceptionistAsync(id.Value);
            if (receptionist == null)
                return NotFound();

            return View(receptionist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,HiringDate,Salary,Shift,Phone,IsActive,Password,Role,Email")] Receptionist receptionist)
        {
            if (id != receptionist.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var result = await _receptionistService.UpdateReceptionistAsync(id, receptionist);
                if (!result.Success)
                    return NotFound();

                TempData["EditMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(receptionist);
        }

        [Authorize(Roles = "AdminDoctor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var receptionist = await _receptionistService.GetReceptionistByIdAsync(id);
            if (receptionist == null)
                return NotFound();

            return View(receptionist);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "AdminDoctor")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _receptionistService.DeleteReceptionistAsync(id);
            if (result.Success)
            {
                TempData["DeleteMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = await _receptionistService.ToggleStatusAsync(id);
            if (!result.Success)
                return NotFound();

            TempData["EditMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}