using Clinic_Management_System.Data;
using Clinic_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]
    public class InternDoctorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InternDoctorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var doctors = from d in _context.InternDoctors
                          select d;

            if (!string.IsNullOrEmpty(search))
                doctors = doctors.Where(d => d.FullName.Contains(search) || d.Phone.Contains(search));

            doctors = doctors.OrderByDescending(d => d.IsActive).ThenBy(d => d.FullName);

            return View(await doctors.ToListAsync());
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
                _context.Add(doctor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(doctor);
        }

        public IActionResult GetById(int id)
        {
            var doctor = _context.InternDoctors
                .Include(d => d.Attendances)
                .FirstOrDefault(m => m.InternDoctorId == id);

            if (doctor == null)
                return NotFound();

            return View(doctor);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _context.InternDoctors.FindAsync(id);
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
                _context.Update(doctor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(doctor);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _context.InternDoctors
                .FirstOrDefaultAsync(d => d.InternDoctorId == id);

            if (doctor == null)
                return NotFound();

            return View(doctor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _context.InternDoctors.FindAsync(id);
            if (doctor == null)
                return NotFound();

            _context.InternDoctors.Remove(doctor);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var doctor = await _context.InternDoctors.FindAsync(id);
            if (doctor == null)
                return NotFound();

            doctor.IsActive = !doctor.IsActive;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult QuickCheckOut(int doctorId)
        {

            var doctor = _context.InternDoctors
                .FirstOrDefault(d => d.InternDoctorId == doctorId && d.IsActive);

            if (doctor == null)
                return NotFound("Doctor not found or inactive.");

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var todayAttendance = _context.InternDoctorAttendances
                .FirstOrDefault(a => a.InternDoctorId == doctorId && a.Date >= today && a.Date < tomorrow);

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

            todayAttendance.CheckOut = DateTime.Now;

            if (todayAttendance.CheckIn != null)
            {
                var duration = (todayAttendance.CheckOut.Value - todayAttendance.CheckIn.Value).TotalHours;
                todayAttendance.Hours = Math.Round(duration, 2);
            }

            _context.SaveChanges();

            TempData["Message"] = $"👋 تم تسجيل انصراف {doctor.FullName} في {DateTime.Now:HH:mm}";
            return RedirectToAction("Index", "InternDoctors");
        }
    }
}
