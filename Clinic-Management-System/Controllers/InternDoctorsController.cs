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
        private readonly IUnitOfWork _unitOfWork;

        public InternDoctorsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var doctors = await _unitOfWork.InternDoctors.GetDoctorsAsync(search);

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
                _unitOfWork.InternDoctors.AddDoctor(doctor);
                await _unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(doctor);
        }

        public IActionResult GetById(int id)
        {
            var doctor = _unitOfWork.InternDoctors.GetDoctorWithAttendances(id);

            if (doctor == null)
                return NotFound();

            return View(doctor);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _unitOfWork.InternDoctors.FindDoctorAsync(id);
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
                _unitOfWork.InternDoctors.UpdateDoctor(doctor);
                await _unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(doctor);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _unitOfWork.InternDoctors.GetDoctorByFilterAsync(id);

            if (doctor == null)
                return NotFound();

            return View(doctor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _unitOfWork.InternDoctors.FindDoctorAsync(id);
            if (doctor == null)
                return NotFound();

            _unitOfWork.InternDoctors.RemoveDoctor(doctor);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var doctor = await _unitOfWork.InternDoctors.FindDoctorAsync(id);
            if (doctor == null)
                return NotFound();

            doctor.IsActive = !doctor.IsActive;
            _unitOfWork.InternDoctors.UpdateDoctor(doctor);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult QuickCheckOut(int doctorId)
        {

            var doctor = _unitOfWork.InternDoctors.GetActiveInternDoctor(doctorId);

            if (doctor == null)
                return NotFound("Doctor not found or inactive.");

            var today = System.DateTime.Today;
            var tomorrow = today.AddDays(1);

            var todayAttendance = _unitOfWork.InternDoctors.GetTodayAttendance(doctorId, today, tomorrow);

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

            _unitOfWork.SaveChanges();

            TempData["Message"] = $"👋 تم تسجيل انصراف {doctor.FullName} في {System.DateTime.Now:HH:mm}";
            return RedirectToAction("Index", "InternDoctors");
        }
    }
}