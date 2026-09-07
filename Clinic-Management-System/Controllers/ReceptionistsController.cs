using Clinic_Management_System.Models;
using Clinic_Management_System.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]

    public class ReceptionistsController : Controller
    {
        private readonly IReceptionistRepository _receptionistRepository;

        public ReceptionistsController(IReceptionistRepository receptionistRepository)
        {
            _receptionistRepository = receptionistRepository;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _receptionistRepository.GetReceptionistsAsync();

            return View(list);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var receptionist = await _receptionistRepository.GetReceptionistByIdAsync(id);
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
                receptionist.HiringDate = System.DateTime.Now;
                receptionist.IsActive = true;
                receptionist.AppointmentLinks = new System.Collections.Generic.List<AppointmentByPatientOrReceptionist>();

                await _receptionistRepository.AddReceptionistAsync(receptionist);

                TempData["SuccessMessage"] = "تمت إضافة موظف الاستقبال بنجاح!";
                return RedirectToAction(nameof(Index));
            }
            return View(receptionist);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var receptionist = await _receptionistRepository.FindReceptionistAsync(id.Value);
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
                try
                {
                    await _receptionistRepository.UpdateReceptionistAsync(receptionist);

                    TempData["EditMessage"] = "تم تعديل بيانات موظف الاستقبال بنجاح!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_receptionistRepository.ReceptionistExists(receptionist.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(receptionist);
        }
        [Authorize(Roles = "AdminDoctor")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var receptionist = await _receptionistRepository.GetReceptionistByIdAsync(id);
            if (receptionist == null)
                return NotFound();

            return View(receptionist);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "AdminDoctor")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var receptionist = await _receptionistRepository.FindReceptionistAsync(id);
            if (receptionist != null)
            {
                await _receptionistRepository.RemoveReceptionistAsync(receptionist);

                TempData["DeleteMessage"] = "تم حذف موظف الاستقبال بنجاح!";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ToggleStatus(int id)
        {
            var receptionist = await _receptionistRepository.FindReceptionistAsync(id);
            if (receptionist == null)
                return NotFound();

            receptionist.IsActive = !receptionist.IsActive;
            await _receptionistRepository.UpdateReceptionistAsync(receptionist);

            TempData["EditMessage"] = receptionist.IsActive
                ? "تم تفعيل الموظف بنجاح!"
                : "تم تعطيل الموظف بنجاح!";

            return RedirectToAction(nameof(Index));
        }
    }
}