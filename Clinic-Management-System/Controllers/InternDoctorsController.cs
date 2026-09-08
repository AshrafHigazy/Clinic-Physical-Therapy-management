using Clinic_Management_System.Models;
using Clinic_Management_System.Services.InternDoctors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]
    public class InternDoctorsController : Controller
    {
        private readonly IInternDoctorService _doctorService;

        public InternDoctorsController(IInternDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var doctors = await _doctorService.GetDoctorsAsync(search);
            return View(doctors);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InternDoctor doctor)
        {
            if (ModelState.IsValid)
            {
                await _doctorService.CreateDoctorAsync(doctor);
                return RedirectToAction(nameof(Index));
            }
            return View(doctor);
        }

        public IActionResult GetById(int id)
        {
            var doctor = _doctorService.GetDoctorWithAttendances(id);
            if (doctor == null)
                return NotFound();

            return View(doctor);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _doctorService.FindDoctorAsync(id);
            if (doctor == null)
                return NotFound();

            return View(doctor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InternDoctor doctor)
        {
            if (id != doctor.InternDoctorId)
                return NotFound();

            if (ModelState.IsValid)
            {
                await _doctorService.UpdateDoctorAsync(id, doctor);
                return RedirectToAction(nameof(Index));
            }
            return View(doctor);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _doctorService.GetDoctorForDeleteAsync(id);
            if (doctor == null)
                return NotFound();

            return View(doctor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _doctorService.DeleteDoctorAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id)
        {
            await _doctorService.ToggleActiveAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult QuickCheckOut(int doctorId)
        {
            var result = _doctorService.QuickCheckOut(doctorId);
            if (result.Message == "Doctor not found or inactive.")
                return NotFound(result.Message);

            TempData["Message"] = result.Message;
            return RedirectToAction("Index", "InternDoctors");
        }
    }
}