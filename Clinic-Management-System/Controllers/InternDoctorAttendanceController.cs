using Clinic_Management_System.Data;
using Clinic_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]

    public class InternDoctorAttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InternDoctorAttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Index - عرض حضور اليوم أو تاريخ معين
        public IActionResult Index(DateTime? date)
        {
            var selectedDate = date ?? DateTime.Today;

            var attendances = _context.InternDoctorAttendances
                .Include(a => a.InternDoctor)
                .Where(a => a.Date.Date == selectedDate.Date)
                .ToList();

            ViewBag.SelectedDate = selectedDate;
            return View(attendances);
        }
        #endregion

        #region Create - إضافة Attendance من صفحة الدكتور أو مستقلة
        [HttpGet]
        public IActionResult Create(int? doctorId)
        {
            var activeDoctors = _context.InternDoctors
                .Where(d => d.IsActive)
                .ToList();

            ViewBag.Doctors = activeDoctors;
            ViewBag.SelectedDoctorId = doctorId;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(InternDoctorAttendance attendance)
        {
            if (!ModelState.IsValid)
                return View(attendance);

            var doctor = _context.InternDoctors
                .FirstOrDefault(d => d.InternDoctorId == attendance.InternDoctorId && d.IsActive);

            if (doctor == null)
            {
                ModelState.AddModelError("", "لا يمكن تسجيل الحضور إلا للدكاترة الفعّالين.");
                return View(attendance);
            }

            bool exists = _context.InternDoctorAttendances
                .Any(a => a.InternDoctorId == attendance.InternDoctorId && a.Date.Date == attendance.Date.Date);

            if (exists)
            {
                ModelState.AddModelError("", "تم تسجيل الحضور لهذا اليوم بالفعل.");
                return View(attendance);
            }

            _context.InternDoctorAttendances.Add(attendance);
            _context.SaveChanges();

            return RedirectToAction("GetById", "InternDoctors", new { id = attendance.InternDoctorId });
            //or retern to attendance bage
            // return RedirectToAction("Index", new { date = attendance.Date });
        }
        #endregion

        #region GetByDoctor - عرض كل الحضور لدكتور معين
        public IActionResult GetByDoctor(int id)
        {
            var doctor = _context.InternDoctors
                .Include(d => d.Attendances)
                .FirstOrDefault(d => d.InternDoctorId == id);

            if (doctor == null)
                return NotFound();

            return View(doctor);
        }
        #endregion

        #region Dashboard - تقرير شامل عن حضور دكتور واحد
        public IActionResult Dashboard(int id, int? month = null, int? year = null, DateTime? from = null, DateTime? to = null)
        {
            var doctor = _context.InternDoctors
                .Include(d => d.Attendances)
                .FirstOrDefault(d => d.InternDoctorId == id);

            if (doctor == null)
                return NotFound();

            DateTime startDate;
            DateTime endDate;

            // ✅ لو المستخدم اختار تواريخ من الفورم
            if (from.HasValue && to.HasValue)
            {
                startDate = from.Value.Date;
                endDate = to.Value.Date;
            }
            // ✅ لو ما اختارش، استخدم الشهر الحالي
            else
            {
                startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
            }

            var filteredAttendances = doctor.Attendances
                .Where(a => a.Date.Date >= startDate && a.Date.Date <= endDate)
                .OrderBy(a => a.Date)
                .ToList();

            var totalDays = filteredAttendances.Count;
            var totalHours = Math.Round(filteredAttendances.Sum(a => a.Hours ?? 0), 2);
            var avgHours = totalDays > 0 ? Math.Round(totalHours / totalDays, 2) : 0;

            ViewBag.TotalDays = totalDays;
            ViewBag.TotalHours = totalHours;
            ViewBag.AvgHours = avgHours;
            ViewBag.DoctorName = doctor.FullName;
            ViewBag.StartDate = startDate.ToShortDateString();
            ViewBag.EndDate = endDate.ToShortDateString();

            return View(filteredAttendances);
        }


        #endregion

        [HttpPost]
        public IActionResult QuickCheckIn(int doctorId)
        {
            var doctor = _context.InternDoctors.FirstOrDefault(d => d.InternDoctorId == doctorId && d.IsActive);
            if (doctor == null)
                return NotFound("Doctor not found or inactive.");

            // لو تم تسجيله النهارده بالفعل
            bool alreadyCheckedIn = _context.InternDoctorAttendances
                .Any(a => a.InternDoctorId == doctorId && a.Date.Date == DateTime.Today);

            if (alreadyCheckedIn)
            {
                TempData["Message"] = "✅ تم تسجيل الحضور بالفعل اليوم.";
                return RedirectToAction("Index", "InternDoctors");
            }

            var attendance = new InternDoctorAttendance
            {
                InternDoctorId = doctorId,
                Date = DateTime.Today,
                CheckIn = DateTime.Now,
                CheckOut = null,
                Hours = null
            };

            _context.InternDoctorAttendances.Add(attendance);
            _context.SaveChanges();

            TempData["Message"] = $"✅ تم تسجيل حضور {doctor.FullName} في {DateTime.Now:HH:mm}";
            return RedirectToAction("Index", "InternDoctors");
        }

        [HttpPost]
        public IActionResult QuickCheckOut(int doctorId)
        {
            var doctor = _context.InternDoctors.FirstOrDefault(d => d.InternDoctorId == doctorId && d.IsActive);
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

            // تسجيل الانصراف الآن
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
