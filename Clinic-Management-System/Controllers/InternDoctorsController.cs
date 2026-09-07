using Clinic_Management_System.Models;
using Clinic_Management_System.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]
    public class InternDoctorsController : Controller
    {
        private readonly IInternDoctorRepository _internDoctorRepository;

        public InternDoctorsController(IInternDoctorRepository internDoctorRepository)
        {
            _internDoctorRepository = internDoctorRepository;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var doctors = await _internDoctorRepository.GetDoctorsAsync(search);

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
                await _internDoctorRepository.AddDoctorAsync(doctor);
                return RedirectToAction(nameof(Index));
            }
            return View(doctor);
        }

        public IActionResult GetById(int id)
        {
            var doctor = _internDoctorRepository.GetDoctorWithAttendances(id);

            if (doctor == null)
                return NotFound();

            return View(doctor);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _internDoctorRepository.FindDoctorAsync(id);
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
                await _internDoctorRepository.UpdateDoctorAsync(doctor);
                return RedirectToAction(nameof(Index));
            }
            return View(doctor);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _internDoctorRepository.GetDoctorByFilterAsync(id);

            if (doctor == null)
                return NotFound();

            return View(doctor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _internDoctorRepository.FindDoctorAsync(id);
            if (doctor == null)
                return NotFound();

            await _internDoctorRepository.RemoveDoctorAsync(doctor);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var doctor = await _internDoctorRepository.FindDoctorAsync(id);
            if (doctor == null)
                return NotFound();

            doctor.IsActive = !doctor.IsActive;
            await _internDoctorRepository.UpdateDoctorAsync(doctor);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult QuickCheckOut(int doctorId)
        {

            var doctor = _internDoctorRepository.GetActiveInternDoctor(doctorId);

            if (doctor == null)
                return NotFound("Doctor not found or inactive.");

            var today = System.DateTime.Today;
            var tomorrow = today.AddDays(1);

            var todayAttendance = _internDoctorRepository.GetTodayAttendance(doctorId, today, tomorrow);

            if (todayAttendance == null)
            {
                TempData["Message"] = "⚠️ لم يتم تسجيل حضور هذا الطبيب اليوم.";
                return RedirectToAction("Index", "InternDoctors");
            }

            if (todayAttendance.CheckOut != null)
            {
                TempData["Message"] = "✅ تم تسجيل الانصراف بالفعل.";
                return RedirectToAction("Index", "InternDoctors");
            }

            todayAttendance.CheckOut = System.DateTime.Now;

            if (todayAttendance.CheckIn != null)
            {
                var duration = (todayAttendance.CheckOut.Value - todayAttendance.CheckIn.Value).TotalHours;
                todayAttendance.Hours = Math.Round(duration, 2);
            }

            _internDoctorRepository.SaveChanges();

            TempData["Message"] = $"👋 تم تسجيل انصراف {doctor.FullName} في {System.DateTime.Now:HH:mm}";
            return RedirectToAction("Index", "InternDoctors");
        }
    }
}